using UnityEngine;
using System.Collections;

/// <summary>
/// A mesh created from a list of vertices
/// Create a mesh of triangles that forms a fan topology
/// </summary>
public class EndCap
{
	Mesh mesh;

	public EndCap(Ring ring)
	{
        this.mesh = CreateCustomMesh(ring.vertices);
	}

    public Mesh GetMesh()
    {
        return this.mesh;
    }

    Mesh CreateCustomMesh(Vector3[] vertices)
    {
        Mesh mesh = new Mesh();

        // Assign the vertices
        mesh.vertices = vertices;

        // Define triangles based on the order of vertices
        int[] triangles = GenerateTriangles(vertices.Length);
        mesh.triangles = triangles;

        // Calculate normals
        mesh.RecalculateNormals();

        return mesh;
    }

    int[] GenerateTriangles(int vertexCount)
    {
        // Generate triangles assuming a fan topology
        int[] triangles = new int[(vertexCount - 2) * 3];
        int index = 0;

        for (int i = 1; i < vertexCount - 1; i++)
        {
            triangles[index++] = 0;
            triangles[index++] = i;
            triangles[index++] = i + 1;
        }

        return triangles;
    }
}