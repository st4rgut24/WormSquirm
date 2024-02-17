using UnityEngine;
using System.Collections;
using System;
using System.Linq;

/// <summary>
/// A mesh created from a list of vertices
/// Create a Cap that has a hole in the center allowing objects to pass through
/// </summary>
public class PassCap
{
    Mesh mesh;

    public PassCap(Ring ring, Ring innerRing)
    {
        this.mesh = CreateCustomMesh(ring.vertices, innerRing.vertices);
    }

    public Mesh GetMesh()
    {
        return this.mesh;
    }

    Mesh CreateCustomMesh(Vector3[] vertices, Vector3[] innerVertices)
    {
        if (vertices.Length != innerVertices.Length)
        {
            throw new Exception("The pass cap rings are not equal in length, so a mesh could not be formed");
        }
        Vector3[] combinedRingVertices = vertices.Concat(innerVertices).ToArray();

        Mesh mesh = new Mesh();
        mesh.vertices = combinedRingVertices;
        mesh.triangles = GenerateTriangles(vertices.Length);

        // Calculate normals
        mesh.RecalculateNormals();

        return mesh;
    }

    /// <summary>
    /// Creates ti
    /// </summary>
    /// <param name="vertexCount"></param>
    /// <returns></returns>
    int[] GenerateTriangles(int vertexCount)
    {
        int[] triangles = new int[6 * vertexCount];

        for (int i = 0, ti = 0; i < vertexCount - 1; i++, ti += 6)
        {
            triangles[ti] = i;
            triangles[ti + 1] = i + vertexCount;
            triangles[ti + 2] = i + 1;

            triangles[ti + 3] = i + 1;
            triangles[ti + 4] = i + vertexCount;
            triangles[ti + 5] = i + vertexCount + 1;
        }

        // Connect the last segment
        triangles[triangles.Length - 6] = vertexCount - 1;
        triangles[triangles.Length - 5] = 2 * vertexCount - 1;
        triangles[triangles.Length - 4] = 0;

        triangles[triangles.Length - 3] = 0;
        triangles[triangles.Length - 2] = 2 * vertexCount - 1;
        triangles[triangles.Length - 1] = vertexCount;

        return triangles;
    }
}