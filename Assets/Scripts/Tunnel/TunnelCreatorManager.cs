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
    /// spawn an obstacle in front of the transform
    /// </summary>
    /// <param name="targetTransform">transform of the object the obstacle should be in front of</param>
    public void SpawnObstacle(Transform targetTransform)
    {
        // Calculate the position in front of the targetTransform
        Vector3 spawnPosition = targetTransform.position + targetTransform.forward;

        // Create a new GameObject (cube) at the calculated position
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = spawnPosition;
        Debug.Log("Spawn cube");

        cube.name = "Cube " + cubeCount;
        // Set the scale of the cube to (1, 1, 1)
        cube.transform.localScale = Vector3.one * 3;

        tunnelGrid.AddGameObject(cube.transform.position, cube);
        cubeCount++;
    }

    /// <summary>
    /// Create a Tunnel segment
    /// </summary>
    /// <param name="playerTransform">The transform of the player</param>
	void CreateAction(Transform playerTransform, Dictionary<Transform, GameObject> NewSegmentDict)
    {
        if (isSpawn)
        {
            SpawnObstacle(playerTransform);
            isSpawn = false;
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

