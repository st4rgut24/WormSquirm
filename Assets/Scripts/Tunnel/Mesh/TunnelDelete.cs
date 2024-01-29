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
        int[] removeFaceIdxArr = HitTriangleIdxSet.ToArray<int>();
        RemoveTriangles(removeFaceIdxArr);
        //this.mesh.RecalculateNormals();

        ComponentUtils.removeMeshCollider(tunnel);

        return removeFaceIdxArr.Length > 0;
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
        Debug.DrawRay(ray.origin, ray.direction * 2f, Color.green, 100.0f);

        if (Physics.Raycast(ray.origin, ray.direction, out hit, 3))
        {
             if (hit.collider.gameObject == tunnel) // if tunnel was the hit gameobject, then add the triangle to removal set
            {
                Debug.DrawRay(ray.origin, ray.direction * 2f, Color.red, 100.0f);
                //Debug.Log("Remove the triangle from Tunnel " + tunnel.name + " at index " + hit.triangleIndex);
                removalIdxSet.Add(hit.triangleIndex);
            }
        }
    }
}
    