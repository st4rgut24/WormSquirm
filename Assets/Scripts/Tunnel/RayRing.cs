using System;
using System.Collections.Generic;
using UnityEngine;

public class RayRing
{
    public List<Ray> rays;
    public Vector3 center;
 
    /// <summary>
    /// Create rays from rings
    /// </summary>
    /// <param name="ringVertexCount">the number of vertices to create per ring</param>
    /// <param name="orientation">orientation of rays</param>
    /// <param name="intersectPos">point of intersection</param>
    /// <param name="maxRadius">upper limit to the distance of rays from center</param>
    /// <param name="offsetMultiple">offset of the center of the ring used to create rays</param>
    /// <param name="radiusInterval">used for determining how many rings to create for rays</param>
    public RayRing(int ringVertexCount, Vector3 orientation, Vector3 intersectPos, int offsetMultiple, float radiusInterval, float maxRadius)
	{
        this.center = intersectPos - orientation * offsetMultiple;

        List<Ring> rings = CreateRings(center, radiusInterval, maxRadius, orientation, ringVertexCount);
        rays = CreateRays(rings, orientation);
	}

    /// <summary>
    /// Create rays from a single ring
    /// </summary>
    /// <param name="ring">a ring</param>
    /// <param name="orientation">direction of rays</param>
    public RayRing(Ring ring, Vector3 orientation)
    {
        rays = CreateRays(new List<Ring> { ring }, orientation);
    }

    public List<Ray> CreateRays(List<Ring> rings, Vector3 orientation)
    {
        List<Ray> rays = new List<Ray>();

        List<Vector3> rayOrigins = GetOrigins(rings);

        rayOrigins.ForEach((origin) =>
        {
            Ray ray = new Ray(origin, orientation);
            rays.Add(ray);
        });

        return rays;
    }


    public List<Ring> CreateRings(Vector3 center, float radiusInterval, float maxRadius, Vector3 orientation, int ringVertexCount)
    {
        List<Ring> rings = new List<Ring>();

        float currentRadius = radiusInterval;

        while (currentRadius <= maxRadius)
        {
            Ring ring = RingFactory.Create(currentRadius, ringVertexCount, orientation, center, null);
            // Increment the radius for the next ring
            currentRadius += radiusInterval;
            rings.Add(ring);
        }

        return rings;
    }

    /// <summary>
    /// Get the origin of rays from rings
    /// </summary>
    /// <returns>origin of rays</returns>
    public List<Vector3> GetOrigins(List<Ring> rings)
    {
        List<Vector3> origins = new List<Vector3>();

        rings.ForEach((ring) =>
        {
            origins.AddRange(ring.vertices);
        });

        return origins;
    }
}

