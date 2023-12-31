using System;
using System.Collections.Generic;
using UnityEngine;

public class TunnelUtils
{
    /// <summary>
    /// Get an intersecting list of gameObjects using bounds
    /// </summary>
    /// <param name="targetObject">the object we are testing intersects</param>
    /// <param name="objectList">list of objects we are testing intersects</param>
    /// <returns></returns>
    public static List<GameObject> GetIntersectedObjects(GameObject targetObject, List<GameObject> objectList)
    {
        List<GameObject> intersectingObjects = new List<GameObject>();

        Bounds targetBounds = targetObject.GetComponent<Renderer>().bounds;

        foreach (GameObject otherObject in objectList)
        {
            if (otherObject != null && otherObject != targetObject) // Ensure the GameObject in the list is not null
            {
                Bounds otherBounds = otherObject.GetComponent<Renderer>().bounds;

                if (targetBounds.Intersects(otherBounds)) // consider using Contains() instead and passing in the targetPosition?
                {
                         intersectingObjects.Add(otherObject); // Bounds intersect with at least one GameObject in the list
                }
                else
                {
                    // Debug.Log("segment " + targetObject.name + " does not intersect object " + otherObject.name);
                }
            }
        }

        return intersectingObjects; // Bounds do not intersect with any GameObject in the list
    }

    /// <summary>
    /// Get the object that contains the targetObject
    /// </summary>
    /// <param name="targetPosition">The target GameObject</param>
    /// <param name="objectList">List of candidate GameObjects that may contain targetObject</param>
    /// <returns></returns>
    public static GameObject getEnclosingObject(Vector3 targetPosition, List<GameObject> objectList)
    {
        foreach (GameObject otherObject in objectList)
        {
            if (otherObject != null) // Ensure the GameObject in the list is not null
            {
                Bounds otherBounds = otherObject.GetComponent<Renderer>().bounds;

                if (otherBounds.Contains(targetPosition))
                {
                    return otherObject;
                }
            }
        }

        return null;
    }

}

