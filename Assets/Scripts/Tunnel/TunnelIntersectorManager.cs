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

    TunnelMake tunnelMaker;
    Grid tunnelGrid;

    TunnelProps _props;

    int _rayInterval; // the smaller the ray interval, the more precise the collision tests during intersection

    private Dictionary<Transform, GameObject> PrevSegmentDict; // maps Player Transform to previous segments

    private void OnEnable()
    {
        TunnelActionManager.OnIntersectTunnel += IntersectAction;
    }

    void Awake()
    {
        tunnelMaker = GameObject.FindObjectOfType<TunnelMake>();
        PrevSegmentDict = new Dictionary<Transform, GameObject>();
    }

    private void Start()
    {
        tunnelGrid = GameManager.Instance.GetGrid(GridType.Tunnel);
        _props = TunnelManager.Instance.defaultProps;
        _rayInterval = Mathf.FloorToInt(_props.TunnelRadius / rayRings);
    }

    /// <summary>
    /// Get the Tunnels that will be intersected
    /// </summary>
    /// <param name="transform">The transform of the tunnel end</param>
    /// <param name="otherTunnels">Other tunnels in the vicinity of the active tunnel</param>
    /// <param name="prevTunnel">Previous tunnel segment belonging to the active tunnel</param>
    /// <param name="lastAction">The last tunnel action taken prior to intersection</param>
	void IntersectAction(Transform transform, GameObject prevTunnel, List<GameObject> otherTunnels, TunnelActionManager.Action lastAction) // todo: restore otherObjects list and call TunnelUtils.GetIntersectedObjects()
    {
        bool isInsideTunnel = lastAction == TunnelActionManager.Action.Follow;
        GameObject projectedSegment = tunnelMaker.GrowTunnel(transform);

        // get intersected tunnels (may be more than 1)
        otherTunnels.Remove(prevTunnel); // adjoining segment does not count as intersected object
        List<GameObject> intersectedTunnels = TunnelUtils.GetIntersectedObjects(projectedSegment, otherTunnels);
        List<Ray> rays = RayUtils.CreateRays(transform, _props.TunnelSegments, _props.TunnelRadius / 2, _rayInterval);


        intersectedTunnels.ForEach((tunnel) =>
        {
            TunnelDelete tunnelDelete = new TunnelDelete(tunnel, rays, isInsideTunnel);
            tunnelDelete.DeleteTunnel();
        });

        if (isInsideTunnel)
        {
            Destroy(projectedSegment); // remove the segment if created inside the tunnel
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDisable()
    {
        TunnelActionManager.OnIntersectTunnel -= IntersectAction;
    }
}

