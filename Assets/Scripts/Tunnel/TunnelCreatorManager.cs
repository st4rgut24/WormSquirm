using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the creation of new tunnels
/// </summary>
public class TunnelCreatorManager : Singleton<TunnelCreatorManager>
{
    public static event Action<Transform, SegmentGo, GameObject> OnAddCreatedTunnel; // <Prev GameObject, Cur GameObject>

    TunnelMake tunnelMaker;

    int cubeCount = 0;

    // testing
    bool isSpawn = false;

    private void OnEnable()
    {
        TunnelActionManager.OnCreateTunnel += CreateAction;
    }

    void Awake()
    {
        tunnelMaker = GameObject.FindObjectOfType<TunnelMake>();
    }

    private void Start()
    {
        //tunnelGrid = GameManager.Instance.GetGrid(GridType.Tunnel);
    }

    // testing
    public void SetSpawnFlag()
    {
        isSpawn = true;
    }

    /// <summary>
    /// Create a Tunnel segment
    /// </summary>
    /// <param name="playerTransform">The transform of the player</param>
    /// <param name="heading">The directional info of the tunnel</param>
	void CreateAction(Transform playerTransform, Heading heading, Ring prevRing)
    {
        GameObject prevSegment = TunnelManager.Instance.GetGameObjectTunnel(playerTransform);
        SegmentManager.Instance.RemovePrevTunnelCap(prevSegment);

        SegmentGo segmentGo = tunnelMaker.GrowTunnel(playerTransform, heading, prevRing);

        OnAddCreatedTunnel?.Invoke(playerTransform, segmentGo, prevSegment);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDisable()
    {
        TunnelActionManager.OnCreateTunnel -= CreateAction;
    }

}

