using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Disables the tunnels that are not near agents for performance
/// </summary>
public class Disabler
{
	Dictionary<Transform, List<GameObject>> ProximalObjectDict;
    int enabledCount;

	public Disabler(int enabledCount)
	{
        this.enabledCount = enabledCount;
		ProximalObjectDict = new Dictionary<Transform, List<GameObject>>();
	}

	/// <summary>
	/// Trigger the disable of surrounding gameobject
	/// </summary>
	public void Disable(Transform transform, Vector3 direction)
	{
        Segment segment = SegmentManager.Instance.GetSegmentFromTransform(transform);

        if (segment == null)
        {
            return;
        }

        List<GameObject> proximalObjects = SearchUtils.bfsSegments(segment, this.enabledCount);

        //List<GameObject> proximalObjects = grid.GetGameObjects(transform.position, this.searchMultiplier);

        List<GameObject> previousProximalObjects = ProximalObjectDict.ContainsKey(transform) ? ProximalObjectDict[transform] : new List<GameObject>();

        // Disable objects that are no longer proximal
        foreach (GameObject previousObject in previousProximalObjects)
        {
            if (!proximalObjects.Contains(previousObject))
            {
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
    }
}

