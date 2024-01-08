using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the creation of new tunnels
/// </summary>
public class TunnelCreatorManager : Singleton<TunnelCreatorManager>
{
    public static event Action<GameObject, GameObject, List<GameObject>> OnAddCreatedTunnel; // <Prev GameObject, Cur GameObject>

    TunnelMake tunnelMaker;
    Grid tunnelGrid;
    List<GameObject> nextTunnels = new List<GameObject>();

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
    /// <param name="lastAction">The action preceding this Create action</param>
	void CreateAction(Transform playerTransform, TunnelActionManager.Action lastAction)
    {
        //if (lastAction == TunnelActionManager.Action.Intersect)
        //{
        //    RingManager.Instance.Remove(playerTransform); // a new segment requires previous ring to be reset
        //}

        GameObject segment = tunnelMaker.GrowTunnel(playerTransform);

        if (segment != null)
        {
            tunnelGrid.AddGameObject(playerTransform.position, segment);

            GameObject prevSegment = TunnelManager.Instance.GetGameObjectSegment(playerTransform);
            OnAddCreatedTunnel?.Invoke(segment, prevSegment, nextTunnels);
        }

        TunnelManager.Instance.AddGameObjectSegment(playerTransform, segment);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

