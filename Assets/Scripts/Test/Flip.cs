using UnityEngine;

public class Flip : MonoBehaviour
{
    void Start()
    {
        // Ensure the GameObject has a MeshFilter component
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter component not found on the GameObject");
            return;
        }

        // Get the mesh from the MeshFilter
        Mesh mesh = meshFilter.mesh;

        // Reverse the order of triangles to flip the faces
        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int temp = triangles[i];
            triangles[i] = triangles[i + 2];
            triangles[i + 2] = temp;
        }

        // Assign the modified triangles back to the mesh
        mesh.triangles = triangles;

        // Recalculate normals to ensure they are correct after flipping faces
        mesh.RecalculateNormals();
    }
}
