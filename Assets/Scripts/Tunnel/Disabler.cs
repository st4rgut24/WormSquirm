using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Disables the tunnels that are not near agents for performance
/// </summary>
public class Disabler
{
    public static event Action<List<GameObject>> OnDisableTunnels;

	public Dictionary<Transform, List<GameObject>> ProximalObjectDict;
    int enabledCount;

	public Disabler(int enabledCount)
	{
        this.enabledCount = enabledCount;
		ProximalObjectDict = new Dictionary<Transform, List<GameObject>>();
	}

	/// <summary>
	/// Trigger the disable of surrounding gameobject
	/// </summary>
    /// . does not include the connection that is made as a result of Dig, called right before the Dig happens
	public void Disable(Transform transform)
	{
        Segment segment = AgentManager.Instance.GetSegment(transform);

        if (segment == null)
        {
            return;
        }

        List<GameObject> disabledTunnels = new List<GameObject>();
        List<GameObject> proximalObjects = SearchUtils.bfsSegments(segment, this.enabledCount);
        List<GameObject> previousProximalObjects = ProximalObjectDict.ContainsKey(transform) ? ProximalObjectDict[transform] : new List<GameObject>();

        // Disable objects that are no longer proximal
        foreach (GameObject previousObject in previousProximalObjects)
        {
            if (!proximalObjects.Contains(previousObject))
            {
                disabledTunnels.Add(previousObject);
                previousObject.SetActive(false);
            }
        }

        // Enable new proximal objects
        foreach (GameObject newObject in proximalObjects)
        {
            if (!newObject.activeSelf)
            {
                newObject.SetActive(true);
            }
        }

        // Update the previousProximalObjects list
        ProximalObjectDict[transform] = proximalObjects;

        // emit event about disabled tunnel
        OnDisableTunnels?.Invoke(disabledTunnels);
    }
}

