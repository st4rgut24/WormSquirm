using System.Collections.Generic;
using UnityEngine;

public class RayUtils
{
    /// <summary>
    /// Create points 
    /// </summary>
    /// <param name="center"></param>
    /// <param name="ringVertexCount"></param>
    /// <param name="radius"></param>
    /// <param name="ringInterval"></param>
    /// <returns></returns>
    public static List<Ray> CreateRays(Transform transform, int ringVertexCount, float radius, float ringInterval)
    {
        List<Ray> rays = new List<Ray>();

        Vector3 offsetCenter = transform.position - transform.forward;// offset so the rays can intersect the mesh

        List<Vector3> ringOrigins = GetRingPoints(transform.forward, offsetCenter, ringVertexCount, radius, ringInterval);

        ringOrigins.ForEach((origin) =>
        {
            Ray ray = new Ray(origin, transform.forward);
            rays.Add(ray);
        });

        return rays;
    }

    private static List<Vector3> GetRingPoints(Vector3 orientation, Vector3 position, int ringVertexCount, float maxRadius, float ringInterval)
    {
        List<Vector3> ringPoints = new List<Vector3>();

        float currentRadius = ringInterval;

        while (currentRadius <= maxRadius)
        {
            Ring ring = RingFactory.get(currentRadius, ringVertexCount, orientation, position, null);

            // Add the points to the overall list
            ringPoints.AddRange(ring.vertices);

            // Increment the radius for the next ring
            currentRadius += ringInterval;
        }

        return ringPoints;
    }
}
