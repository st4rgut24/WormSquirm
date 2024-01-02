using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Delete the faces of a mesh that intersect with the direction of a Transform
/// </summary>
public class TunnelDelete
{
    List<Ray> rays;
    Mesh mesh;
    HashSet<int> removeIdxSet;

    public TunnelDelete(Mesh mesh, List<Ray> rays)
    {
        this.mesh = mesh;
        this.rays = rays;
    }

    public void DeleteInvertedFaces(GameObject tunnel)
    {
        FlipNormals();
        tunnel.AddComponent<MeshCollider>();
        DeleteFaces();
        //FlipNormals();
    }

    /// <summary>
    /// Deletes faces using a collider test, inverting faces if necessary to test collision
    /// </summary>
    /// <param name="invertFaces">flag to invert faces</param>
    public void DeleteFaces()
    {
        HashSet<int> HitTriangleIdxSet = GetTrianglesHitByRays(this.mesh, this.rays);
        int[] removeFaceIdxArr = HitTriangleIdxSet.ToArray<int>();
        RemoveTriangles(removeFaceIdxArr);
        //this.mesh.RecalculateNormals();
    }

    void RemoveTriangles(int[] trianglesToRemove)
    {
        // Create a new triangle array without the specified triangles
        int[] newTriangles = new int[mesh.triangles.Length - trianglesToRemove.Length * 3];
        int currentIndex = 0;

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            // Check if the current triangle should be removed
            if (System.Array.IndexOf(trianglesToRemove, i / 3) == -1)
            {
                // Copy the vertices of the triangle to the new array
                newTriangles[currentIndex++] = mesh.triangles[i];
                newTriangles[currentIndex++] = mesh.triangles[i + 1];
                newTriangles[currentIndex++] = mesh.triangles[i + 2];
            }
        }

        // Set the new triangle array to the mesh
        mesh.triangles = newTriangles;
    }

    HashSet<int> GetTrianglesHitByRays(Mesh mesh, List<Ray> rays)
    {
        HashSet<int> triangleRemoveSet = new HashSet<int>();

        for (int i = 0; i < rays.Count; i++)
        {
            Ray ray = rays[i];
            GetTrianglesHitByRay(ray, triangleRemoveSet);
        }  

        return triangleRemoveSet;
    }

    void GetTrianglesHitByRay(Ray ray, HashSet<int> removalIdxSet)
    {
        RaycastHit hit;

        // Draw the ray in the Scene view regardless of whether it hits anything
        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 10.0f);

        if (Physics.Raycast(ray.origin, ray.direction, out hit))
        {
            Debug.Log("remove triangle at index " + hit.triangleIndex);
            Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.green, 10.0f);

            removalIdxSet.Add(hit.triangleIndex);
        }
    }

    /// <summary>
    /// Flip the normals of a mesh to test collision on the inside faces
    /// </summary>
    /// <param name="mesh">Mesh</param> 
    void FlipNormals()
    {
        mesh.triangles = mesh.triangles.Reverse().ToArray();
    }
}
    