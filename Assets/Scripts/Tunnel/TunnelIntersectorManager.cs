using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the intersection of tunnels
/// </summary>
public class TunnelIntersectorManager : Singleton<TunnelIntersectorManager>
{
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
    /// <param name="prevPos">Previoous position of the player </param>
    /// <param name="lastAction">The last tunnel action taken prior to intersection</param>
	void IntersectAction(Transform transform, GameObject prevTunnel, Vector3 prevPos, List<GameObject> otherTunnels, TunnelActionManager.Action lastAction) // todo: restore otherObjects list and call TunnelUtils.GetIntersectedObjects()
    {
        bool isInsideTunnel = lastAction == TunnelActionManager.Action.Follow;

        if (isInsideTunnel)
        {
            // use prev position of transform to get the last position INSIDE the tunnel
            // pass this in to the functions below
            Ring insideRing = RingManager.Instance.Create(transform.forward, prevPos);
            RingManager.Instance.UpdateEntry(transform, insideRing);
        }

        Intersect(transform, prevTunnel, otherTunnels, isInsideTunnel);
    }

    void Intersect(Transform transform, GameObject prevTunnel, List<GameObject> otherTunnels, bool isInsideTunnel)
    {
        GameObject projectedSegment = tunnelMaker.GrowTunnel(transform);

        // get intersected tunnels (may be more than 1)
        otherTunnels.Remove(prevTunnel); // adjoining segment does not count as intersected object
        List<GameObject> intersectedTunnels = TunnelUtils.GetIntersectedObjects(projectedSegment, otherTunnels);
        List<Ray> rays = RayUtils.CreateRays(transform, _ringVertices, _holeRadius, _rayInterval, offsetMultiple); // experiment with TunnelRadius, rayIntervals

        intersectedTunnels.ForEach((tunnel) =>
        {
            TunnelDelete tunnelDelete = TunnelDeleteFactory.Get(tunnel, rays, isInsideTunnel);
            tunnelDelete.DeleteTunnel();
        });
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