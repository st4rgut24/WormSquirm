using System;
using System.Collections.Generic;
using System.Linq;
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
                    // Debug.Log("segment " + targetObject.name + " does not intersect object " + otherObject.name);
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

    public static HitInfo GetHitInfoFromRay(Ray ray, GameObject go, float rayDistance)
    {
        return GetHitInfoFromRays(new List<Ray>() { ray }, new List<GameObject> { go }, rayDistance);
    }

    public static HitInfo GetHitInfoFromRays(List<Ray> rays, List<GameObject> objectList, float rayDistance)
    {
        RaycastHit hit;
        ComponentUtils.addBoxColliders(objectList);

        HitInfo hitInfo = null;

        //Ray ray = new Ray(transform.position, transform.forward);

        for (int i = 0; i < rays.Count; i++)
        {
            Ray ray = rays[i];

            bool didHit = Physics.Raycast(ray.origin, ray.direction, out hit, rayDistance);

            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.cyan, 200.0f);
            if (didHit)
            {
                Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red, 200.0f);
                hitInfo = new HitInfo(hit.collider.gameObject, hit.point);

                if (!objectList.Contains(hitInfo.hitGo))
                {
                    throw new Exception("A gameobject other than the ones provided in list was hit.");
                }
                break;
            }
        }

        ComponentUtils.removeBoxColliders(objectList);

        return hitInfo;
    }

}

