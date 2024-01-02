using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the creation of new tunnels
/// </summary>
public class TunnelCreatorManager : Singleton<TunnelCreatorManager>
{
    TunnelMake tunnelMaker;
    Grid tunnelGrid;

    private Dictionary<Transform, GameObject> PrevSegmentDict; // maps Player Transform to previous segments

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
        PrevSegmentDict = new Dictionary<Transform, GameObject>();
    }

    private void Start()
    {
        tunnelGrid = GameManager.Instance.GetGrid(GridType.Tunnel);
    }

    // testing
    public void SetSpawnFlag()
    {
        // Debug.Log("Set spawn flag");
        isSpawn = true;
    }

    /// <summary>
    /// Create a Tunnel segment
    /// </summary>
    /// <param name="playerTransform">The transform of the player</param>
    /// <param name="NewSegmentDict">Mapping of new segments</param>
    /// <param name="lastAction">The action preceding this Create action</param>
	void CreateAction(Transform playerTransform, Dictionary<Transform, GameObject> NewSegmentDict, TunnelActionManager.Action lastAction)
    {
        if (lastAction == TunnelActionManager.Action.Intersect)
        {
            tunnelMaker.clearPrevRingEntry(playerTransform); // a new segment requires previous ring to be reset
        }

        GameObject segment = tunnelMaker.GrowTunnel(playerTransform);

        if (segment != null)
        {
            tunnelGrid.AddGameObject(playerTransform.position, segment);
        }

        NewSegmentDict[playerTransform] = segment;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

