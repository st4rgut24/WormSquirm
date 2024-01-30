using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements.Experimental;
using static UnityEngine.Rendering.HableCurve;

/// <summary>
/// Manages the intersection of tunnels
/// </summary>
public class TunnelIntersectorManager : Singleton<TunnelIntersectorManager>
{
    public static event Action<Transform, SegmentGo, GameObject, List<GameObject>> OnAddIntersectedTunnel;

    public int rayRings = 3; // The bigger this number is, the smaller the interval
    public int offsetMultiple = 3; // How many units the intersection ray should be offset from the intersecting faces

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
    /// <param name="extendsTunnel">The player is extending a tunnel</param>
    /// <param name="heading">The directional info of the tunnel</param>
    void IntersectAction(Transform transform, GameObject prevTunnel, Heading heading, List<GameObject> otherTunnels, bool extendsTunnel, Ring prevRing)
    {
        List<Ray> endRingRays = RayUtils.CreateRays(heading.position, -heading.forward, _ringVertices, _holeRadius, _rayInterval, offsetMultiple); // experiment with TunnelRadius, rayIntervals

        List<Ray> rays = new List<Ray>(endRingRays);

        if (!extendsTunnel) // tunnel faces may be deleted from the start of a segment if the intersecting segment bisects the current segment
        {
            List<Ray> startRingRays = RayUtils.CreateRays(prevRing.GetCenter(), heading.forward, _ringVertices, _holeRadius, _rayInterval, offsetMultiple); // experiment with TunnelRadius, rayIntervals
            rays.AddRange(startRingRays);
        }

        Intersect(transform, prevTunnel, otherTunnels, extendsTunnel, rays, heading, prevRing);
    }

    void Intersect(Transform transform, GameObject prevTunnel, List<GameObject> otherTunnels, bool extendsTunnel, List<Ray> rays, Heading heading, Ring prevRing)
    {
        SegmentGo projectedSegment = tunnelMaker.GrowTunnel(transform, heading, extendsTunnel, prevRing);


        // get intersected tunnels (may be more than 1)

        // TODO: Removing this line may be a bad idea, but previous tunnel may also be intersected as a result of creating a bisecting tunnel out of the current segment
        //otherTunnels.Remove(prevTunnel); // adjoining segment does not count as intersected object

        List<GameObject> intersectedTunnels = TunnelUtils.GetIntersectedObjects(projectedSegment.getTunnel(), otherTunnels);

        List<GameObject> deletedTunnels = new List<GameObject>();

        intersectedTunnels.ForEach((tunnel) =>
        {
            TunnelDelete tunnelDelete = new TunnelDelete(tunnel, rays);
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