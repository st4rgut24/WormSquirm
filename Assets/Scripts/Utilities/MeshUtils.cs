using System;
using System.Linq;
using UnityEngine;

public class MeshUtils
{
    public static int[] FlipNormals(Mesh mesh)
    {
        return mesh.triangles.Reverse().ToArray();
    }

    public static int[] ConvertTriangleIdxToVertexArr(int[] triangleIdxArr)
    {
        int[] vertexIdxArr = new int[triangleIdxArr.Length];

        for (int i = 0; i < triangleIdxArr.Length; i++)
        {
            vertexIdxArr[i] = triangleIdxArr[i] * 3;
        }

        return vertexIdxArr;
    }

    public static float GetSideLength(int sides, float radius)
    {
        float angleInRadians = Mathf.PI / sides;
        float sideLength = 2 * radius * Mathf.Sin(angleInRadians);
        return sideLength;
    }

    public static void InvertFaces(Mesh mesh)
    {
        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -normals[i];
        }
        mesh.normals = normals;

        // Flip triangles to maintain winding order
        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int temp = triangles[i + 1];
            triangles[i + 1] = triangles[i + 2];
            triangles[i + 2] = temp;
        }
        mesh.triangles = triangles;
    }
}

