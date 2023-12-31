using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the intersection of tunnels
/// </summary>
public class TunnelIntersectorManager : Singleton<TunnelIntersectorManager>
{
    TunnelMake tunnelMaker;
    Grid tunnelGrid;

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
    }

    /// <summary>
    /// Get the Tunnels that will be intersected
    /// </summary>
    /// <param name="playerTransform">The transform of the player</param>
	void IntersectAction(Transform playerTransform, List<GameObject> otherTunnels, GameObject prevSegment)
    {
        // 1. Create a Tunnel
        GameObject segment = tunnelMaker.GrowTunnel(playerTransform); 

        otherTunnels.Remove(prevSegment); // adjoining segment does not count as intersected object
        List<GameObject> intersectedTunnels = TunnelUtils.GetIntersectedObjects(segment, otherTunnels);

        // 2. Get the intersecting GameObjects
        if (intersectedTunnels.Count > 0)
        {
            Debug.Log("Tunnel Intersection Detected!");
            intersectedTunnels.ForEach((tunnel) =>
            {
                Debug.Log("Intersects tunnel " + tunnel.name);
                //GameObject.Destroy(tunnel);
            });

        }

        // TODO: 
        // 3. Get the intersecting Faces
        // 4. Delete the intersecting Faces
        // 5. Create a Ring segment to best fill the hole
        // 6. Create a new Tunnel segment that connects to the ring segment occupying the hole

        PrevSegmentDict[playerTransform] = segment;
        GameObject.Destroy(segment);
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

