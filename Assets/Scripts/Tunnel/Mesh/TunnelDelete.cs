using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Delete the faces of a mesh that intersect with the direction of a Transform
/// </summary>
public class TunnelDelete
{
    public static void DeleteFacesHitByRays(Mesh mesh, List<Ray> rays)
    {
        for (int i = 0; i < rays.Count; i++)
        {
            Ray ray = rays[i];
            DeleteFaceHitByRay(mesh, mesh.triangles, ray);
        }
    }

    public static void DeleteFaceHitByRay(Mesh mesh, int[] triangles, Ray ray)
    {
        RaycastHit hit;

        // Draw the ray in the Scene view regardless of whether it hits anything
        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 1.0f);

        if (Physics.Raycast(ray.origin, ray.direction, out hit))
        {
            Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.green, 1.0f);

            triangles = triangles.Where((t, i) => i < hit.triangleIndex * 3 || i >= (hit.triangleIndex + 1) * 3).ToArray();
            mesh.triangles = triangles;
        }
    }
}
