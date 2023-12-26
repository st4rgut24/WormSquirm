using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Manages locations of existing tunnel, uses player position to determine
/// whether to create a new tunnel
/// </summary>
public class TunnelManager : MonoBehaviour
{
    TunnelMake tunnelMaker;
    Grid tunnelGrid;

    private void OnEnable()
    {
        Player.OnMove += TunnelAction;
    }

    void Awake()
	{
        tunnelMaker = GameObject.FindObjectOfType<TunnelMake>();
        tunnelGrid = new Grid();
	}

	void TunnelAction(Transform transform)
	{
        // if tunnel does not exist yet, create it
        if (tunnelGrid.hasGameObjects(transform.position))
        {
            // todo: connect intersecting tunnel segments using some Bounds.Intersects logic and dynamic colliders potentially.
        }
        else
        {
            GameObject tunnelGO = tunnelMaker.GrowTunnel(transform);
            tunnelGrid.addGameObject(transform.position, tunnelGO);
        }
    }

	// Update is called once per frame
	void Update()
	{
			
	}

    private void OnDisable()
    {
        Player.OnMove -= TunnelAction;
    }
}

