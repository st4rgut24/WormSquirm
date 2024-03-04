using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Delete the faces of a mesh that intersect with the direction of a Transform
/// </summary>
public class TunnelDelete
{
    protected List<Ray> rays;
    protected Mesh mesh;
    protected GameObject tunnel;

    HashSet<int> removeIdxSet;

    public TunnelDelete(GameObject Tunnel, List<Ray> rays)
    {
        this.tunnel = Tunnel;
        this.mesh = ComponentUtils.GetMesh(Tunnel);
        this.rays = rays;
    }

    /// <summary>
    /// Deletes faces using a collider test, inverting faces if necessary to test collision
    /// </summary>
    /// <param name="invertFaces">flag to invert faces</param>
    /// returns true if deletion occurs
    public virtual bool DeleteTunnel()
    {
        tunnel.AddComponent<MeshCollider>();

        HashSet<int> HitTriangleIdxSet = GetTrianglesHitByRays(this.mesh, this.rays);
        int[] removeTriangleIdxArr = HitTriangleIdxSet.ToArray<int>();
        int[] removeVertexArr = MeshUtils.ConvertTriangleIdxToVertexArr(removeTriangleIdxArr);
        RemoveTriangles(removeVertexArr);
        //this.mesh.RecalculateNormals();

        ComponentUtils.removeMeshCollider(tunnel);

        return removeTriangleIdxArr.Length > 0;
    }

    void RemoveTriangles(int[] verticesToRemove)
    {
        // Create a new triangle array without the specified triangles
        int[] newTriangles = new int[mesh.triangles.Length - verticesToRemove.Length * 3];
        int currentIndex = 0;

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            // If the current triangle is not removed, then add it to the new triangles array
            // only include the vertices of triangles that are NOT removed
            if (System.Array.IndexOf(verticesToRemove, i) == -1)
            {
                // If the vertex of the triangle is not among the removed vertices,
                // copy the vertices of the triangle to the new array
                newTriangles[currentIndex++] = mesh.triangles[i];
                newTriangles[currentIndex++] = mesh.triangles[i + 1];
                newTriangles[currentIndex++] = mesh.triangles[i + 2];
            }
        }

        // Set the new triangle array to the mesh
        mesh.triangles = newTriangles;
    }

    /// <summary>
    /// Get the indices of triangles that have been intersected
    /// </summary>
    /// <param name="mesh">mesh to test for intersection</param>
    /// <param name="rays">intersecting rays</param>
    /// <returns>set of triangle indices, each indices representing the first vertex of a triangle</returns>
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
        Debug.DrawRay(ray.origin, ray.direction * 6f, Color.green, 100.0f);

        if (Physics.Raycast(ray.origin, ray.direction, out hit, 6))
        {
             if (hit.collider.gameObject == tunnel && hit.triangleIndex != -1) // if tunnel was the hit gameobject, then add the triangle to removal set
            {
                Debug.DrawRay(ray.origin, ray.direction * 6f, Color.red, 100.0f);
                //// Debug.Log("Remove the triangle from Tunnel " + tunnel.name + " at index " + hit.triangleIndex);
                removalIdxSet.Add(hit.triangleIndex);
            }
        }
    }
}
    