using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages locations of existing tunnel, uses player position to determine
/// whether to create a new tunnel
/// </summary>
public class TunnelManager : MonoBehaviour
{
    public static event Action<GameObject, List<GameObject>> OnIntersectTunnel;

    TunnelMake tunnelMaker;
    Grid tunnelGrid;

    GameObject prevSegment;
    private Dictionary<Transform, GameObject> PrevSegmentDict; // maps Player Transform to previous segments

    int cubeCount = 0;

    // testing
    bool isSpawn = false;

    private void OnEnable()
    {
        Agent.OnMove += TunnelAction;
    }

    void Awake()
	{
        tunnelMaker = GameObject.FindObjectOfType<TunnelMake>();
        tunnelGrid = new Grid();
        PrevSegmentDict = new Dictionary<Transform, GameObject>();
    }

    // testing
    public void SetSpawnFlag()
    {
        Debug.Log("Set spawn flag");
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
    /// take an action based on the location of the next tunnel segment
    /// </summary>
    /// <param name="projectedTransform">The projected location of the tunnel</param>
	void TunnelAction(Transform projectedTransform)
	{
        List<GameObject> otherTunnels = tunnelGrid.GetGameObjects(projectedTransform.position);
        GameObject projectedSegment = addNewTunnel(projectedTransform);

        // if tunnel does not exist yet, create it
        if (otherTunnels != null)
        {
            otherTunnels.Remove(PrevSegmentDict[projectedTransform]); // adjoining segment does not count as intersected object
            List<GameObject> intersectedTunnels = CollisionUtils.getIntersectedObjects(projectedSegment, otherTunnels);

            if (intersectedTunnels.Count > 0)
            {
                Debug.Log("Tunnel Intersection Detected!");
                OnIntersectTunnel?.Invoke(projectedSegment, intersectedTunnels);
            }
        }

        PrevSegmentDict[projectedTransform] = projectedSegment;
    }

    GameObject addNewTunnel(Transform transform)
    {
        if (isSpawn)
        {
            SpawnObstacle(transform);
            isSpawn = false;
        }

        GameObject segment = tunnelMaker.GrowTunnel(transform);

        if (segment != null)
        {
            tunnelGrid.AddGameObject(transform.position, segment);
        }
       
        return segment;
    }

	// Update is called once per frame
	void Update()
	{
			
	}

    private void OnDisable()
    {
        Agent.OnMove -= TunnelAction;
    }
}

