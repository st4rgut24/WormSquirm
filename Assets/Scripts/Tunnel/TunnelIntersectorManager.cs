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
	void IntersectAction(Transform transform, GameObject prevTunnel, List<GameObject> otherTunnels) // todo: restore otherObjects list and call TunnelUtils.GetIntersectedObjects()
    {
        GameObject projectedSegment = tunnelMaker.GrowTunnel(transform);

        // get intersected tunnels (may be more than 1)
        otherTunnels.Remove(prevTunnel); // adjoining segment does not count as intersected object
        List<GameObject> intersectedTunnels = TunnelUtils.GetIntersectedObjects(projectedSegment, otherTunnels);

        // 1. Create Rays with player forward vector
        List<Ray> rays = RayUtils.CreateRays(transform, _props.TunnelSegments, _props.TunnelRadius / 2, _rayInterval);

        // 2. Attach MeshCollider to intersected tunnel
        ComponentUtils.addMeshColliders(intersectedTunnels);

        // 3. Get the intersecting Faces
        // 4. Delete the intersecting Faces
        // Todo: test
        DeleteIntersectedFaces(intersectedTunnels, rays);

        // 5. Create a Ring segment to best fill the hole
        // 6. Create a new Tunnel segment that connects to the ring segment occupying the hole

        ComponentUtils.removeMeshColliders(intersectedTunnels);
    }

     void DeleteIntersectedFaces(List<GameObject> tunnels, List<Ray> rays)
    { 
        List<Mesh> meshes = ComponentUtils.GetMeshes(tunnels);

        meshes.ForEach((mesh) =>
        {
            TunnelDelete tunnelDelete = new TunnelDelete(mesh, rays);
            tunnelDelete.DeleteFaces();
        });
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

