using System;
using System.Collections.Generic;
using UnityEngine;

public class CollisionUtils
{

    //public static bool IsIntersectingObjectValid(GameObject targetObject, GameObject otherObject)
    //{
    //    if (otherObject != null && otherObject != targetObject)
    //    {
    //        Vector3 dir = otherObject.transform.position - targetObject.transform.position;

    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    /// <summary>
    /// Get an intersecting list of gameObjects using bounds
    /// </summary>
    /// <param name="targetObject">the object we are testing intersects</param>
    /// <param name="objectList">list of objects we are testing intersects</param>
    /// <returns></returns>
    public static List<GameObject> getIntersectedObjects(GameObject targetObject, List<GameObject> objectList)
    {
        List<GameObject> intersectingObjects = new List<GameObject>();

        Bounds targetBounds = targetObject.GetComponent<Renderer>().bounds;

        foreach (GameObject otherObject in objectList)
        {
            if (otherObject != null && otherObject != targetObject) // Ensure the GameObject in the list is not null
            {
                Bounds otherBounds = otherObject.GetComponent<Renderer>().bounds;

                if (targetBounds.Intersects(otherBounds))
                {
                         intersectingObjects.Add(otherObject); // Bounds intersect with at least one GameObject in the list
                }
                else
                {
                    Debug.Log("segment " + targetObject.name + " does not intersect object " + otherObject.name);
                }
            }
        }

        return intersectingObjects; // Bounds do not intersect with any GameObject in the list
    }
}

