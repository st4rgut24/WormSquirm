using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the intersection of tunnels
/// </summary>
public class TunnelIntersectorManager : Singleton<TunnelIntersectorManager>
{
    public static event Action<Transform, SegmentGo, GameObject, List<GameObject>> OnAddIntersectedTunnel;

    public int rayRings = 3; // The bigger this number is, the smaller the interval
    public int offsetMultiple = 2; // How many units the intersection ray should be offset from the intersecting faces

    TunnelMake tunnelMaker;
    Grid tunnelGrid;

    TunnelProps _props;
    int _ringVertices;
    float _holeRadius;

    int _rayInterval; // the smaller the ray interval, the more precise the collision tests during intersection

    private void OnEnable()
    {
        TunnelActionManager.OnIntersectTunnel += IntersectAction;
    }

    void Awake()
    {
        tunnelMaker = GameObject.FindObjectOfType<TunnelMake>();

    }

    private void Start()
    {
        tunnelGrid = GameManager.Instance.GetGrid(GridType.Tunnel);
        _props = TunnelManager.Instance.defaultProps;
        _rayInterval = Mathf.FloorToInt(_props.TunnelRadius / rayRings);

        _ringVertices = _props.TunnelSegments + 5;
        _holeRadius = _props.TunnelRadius / 2;
    }

    /// <summary>
    /// Get the Tunnels that will be intersected
    /// </summary>
    /// <param name="transform">The transform of the tunnel end</param>
    /// <param name="otherTunnels">Other tunnels in the vicinity of the active tunnel</param>
    /// <param name="prevTunnel">Previous tunnel segment belonging to the active tunnel</param>
    /// <param name="prevHeading">Previous transform info of the player </param>
    /// <param name="lastAction">The last tunnel action taken prior to intersection</param>
    /// <param name="heading">The directional info of the tunnel</param>
    void IntersectAction(Transform transform, GameObject prevTunnel, Heading heading, Heading prevHeading, List<GameObject> otherTunnels, TunnelActionManager.Action lastAction) // todo: restore otherObjects list and call TunnelUtils.GetIntersectedObjects()
    {
        List<Ray> rays;

        bool isInsideTunnel = lastAction == TunnelActionManager.Action.Follow;

        if (isInsideTunnel)
        {
            // use prev position of transform to get the last position INSIDE the tunnel
            // pass this in to the functions below
            Ring insideRing = RingManager.Instance.Create(heading.forward, prevHeading.position);
            RingManager.Instance.UpdateEntry(transform, insideRing);
        }
        rays = RayUtils.CreateRays(heading.position, -heading.forward, _ringVertices, _holeRadius, _rayInterval, offsetMultiple); // experiment with TunnelRadius, rayIntervals

        Intersect(transform, prevTunnel, otherTunnels, isInsideTunnel, rays, heading);
    }

    void Intersect(Transform transform, GameObject prevTunnel, List<GameObject> otherTunnels, bool isInsideTunnel, List<Ray> rays, Heading heading)
    {
        SegmentGo projectedSegment = tunnelMaker.GrowTunnel(transform, heading, isInsideTunnel);

        // get intersected tunnels (may be more than 1)
        otherTunnels.Remove(prevTunnel); // adjoining segment does not count as intersected object
        List<GameObject> intersectedTunnels = TunnelUtils.GetIntersectedObjects(projectedSegment.segment, otherTunnels);

        List<GameObject> deletedTunnels = new List<GameObject>();

        intersectedTunnels.ForEach((tunnel) =>
        {
            TunnelDelete tunnelDelete = TunnelDeleteFactory.Get(tunnel, rays, isInsideTunnel);
            bool isDeleted = tunnelDelete.DeleteTunnel();

            if (isDeleted)
            {
                deletedTunnels.Add(tunnel);
            }
        });

        OnAddIntersectedTunnel?.Invoke(transform, projectedSegment, prevTunnel, deletedTunnels);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDisable()
    {
        TunnelActionManager.OnIntersectTunnel -= IntersectAction;
    }
}