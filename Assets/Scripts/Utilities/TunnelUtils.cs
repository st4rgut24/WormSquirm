using System;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class TunnelUtils
{
    private static Bounds GetAdjustedBounds(GameObject tunnelGo, float buffer)
    {
        Bounds originalBounds = tunnelGo.GetComponent<Renderer>().bounds;
        // Adjust the bounds with the buffer value
        Vector3 sizeWithBuffer = originalBounds.size + new Vector3(buffer, buffer, buffer) * 2f;
        Bounds adjustedBounds = new Bounds(originalBounds.center, sizeWithBuffer);

        return adjustedBounds;
    }

    /// <summary>
    /// Get an intersecting list of gameObjects using bounds
    /// </summary>
    /// <param name="targetObject">the object we are testing intersects</param>
    /// <param name="objectList">list of objects we are testing intersects</param>
    /// <param name="intersectBuffer">buffer to adjust bounds of intersecting objects</param>
    /// <returns></returns>
    public static List<GameObject> GetIntersectedObjects(GameObject targetObject, List<GameObject> objectList, float intersectBuffer = 0)
    {
        List<GameObject> intersectingObjects = new List<GameObject>();

        Bounds targetBounds = GetAdjustedBounds(targetObject, intersectBuffer);

        foreach (GameObject otherObject in objectList)
        {
            if (otherObject != null && otherObject != targetObject) // Ensure the GameObject in the list is not null
            {
                Bounds otherBounds = GetAdjustedBounds(otherObject, intersectBuffer);

                if (targetBounds.Intersects(otherBounds)) // consider using Contains() instead and passing in the targetPosition?
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

    public static Vector3 GetCenterPoint(Vector3 point1, Vector3 point2)
    {
        return (point1 + point2) / 2;
    }

    /// <summary>
    /// Get the object closest to position
    /// </summary>
    /// <param name="targetPosition">The target GameObject</param>
    /// <param name="objectList">List of candidate GameObjects that may contain targetObject</param>
    /// <returns></returns>
    public static GameObject getClosestObject(Vector3 targetPosition, List<GameObject> objectList)
    {
        float closestDist = Mathf.Infinity;
        GameObject closestTunnel = null;

        foreach (GameObject otherObject in objectList)
        {
            if (otherObject != null) // Ensure the GameObject in the list is not null
            {
                Bounds otherBounds = otherObject.GetComponent<Renderer>().bounds;

                float dist = Vector3.Distance(otherBounds.center, targetPosition);

                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestTunnel = otherObject;
                }
            }
        }

        return closestTunnel;
    }

    public static List<GameObject> GetAdjoiningTunnels(Segment segment)
    {
        List<GameObject> TunnelGoList = new List<GameObject>();
        TunnelGoList.Add(segment.tunnel);

        TunnelGoList.AddRange(segment.getNextTunnels());

        return TunnelGoList;
    }

    /// <summary>
    /// Get the object that contains the targetObject
    /// </summary>
    /// <param name="targetPosition">The target GameObject</param>
    /// <param name="objectList">List of candidate GameObjects that may contain targetObject</param>
    /// <returns>null if no enclosing object</returns>
    //public static GameObject getEnclosingObject(Vector3 targetPosition, List<GameObject> objectList)
    //{
    //    GameObject enclosingObject = null;

    //    foreach (GameObject otherObject in objectList)
    //    {
    //        if (otherObject != null) // Ensure the GameObject in the list is not null
    //        {
    //            Bounds otherBounds = otherObject.GetComponent<Renderer>().bounds;

    //            if (otherBounds.Contains(targetPosition))
    //            {
    //                if (enclosingObject == null)
    //                {
    //                    enclosingObject = otherObject;
    //                }
    //                else
    //                {
    //                    throw new Exception("There is more than one enclosing object for a transform.position " + targetPosition);
    //                }
    //            }
    //        }
    //    }

    //    return enclosingObject;
    //}

    public static HitInfo getHitObject(Transform transform, List<GameObject> objectList)
    {
        RaycastHit hit;
        ComponentUtils.addBoxColliders(objectList);

        Ray ray = new Ray(transform.position, transform.forward);

        bool didHit = Physics.Raycast(ray.origin, ray.direction, out hit, GameManager.Instance.agentOffset);

        ComponentUtils.removeBoxColliders(objectList);

        if (didHit)
        {
            Debug.DrawRay(ray.origin, ray.direction * GameManager.Instance.agentOffset, Color.cyan, 200.0f);
            return new HitInfo(hit.collider.gameObject, hit.point);
        }
        else
        {
            return null;
        }
    }

}

