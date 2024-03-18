using System;
using System.Collections.Generic;
using UnityEngine;

public class Ring
{
    const float defaultNoise = 0;

    public Vector3[] vertices;
    public float radius;
    public Vector3 normal;
    public Vector3 center;

    List<Guideline> vertexLineList;

    public Ring(float radius, int vertexCount, Vector3 normal, Vector3 center, float? noiseScale)
	{
        this.radius = radius;
        this.vertices = new Vector3[vertexCount];
        this.center = center;
        this.normal = normal;

        vertexLineList = new List<Guideline>();
        // Debug.Log("Center is " + center);
        float angularOffset = GetAngularOffset(vertexCount);
        SetVertices(vertexCount, normal, angularOffset, noiseScale);
        SetRingGuidelines(vertices, center);
    }

    /// <summary>
    /// Get the number of degrees to rotate the shape so that its side is parallel with the ground
    /// </summary>
    /// <param name="vertexCount">vertices or sides per shape</param>
    /// <returns>offset angle</returns>
    public float GetAngularOffset(int vertexCount)
    {
        float anglePerSide = Consts.FullRevolution / vertexCount;

        return anglePerSide / 2;
    }

    public List<Guideline> GetRingLines()
    {
        return vertexLineList;
    }

    public Vector3 GetCenter()
    {
        return center;
    }

    public Vector3 GetNormal()
    {
        return normal;
    }

    /// <summary>
    /// Set guidelines defining the plane of the ring
    /// guidelines radiate from the center to each vertex of the ring
    /// </summary>
    /// <param name="vertices">vertices of the ring</param>
    void SetRingGuidelines(Vector3[] vertices, Vector3 center)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];
            Guideline guideline = new Guideline(center, vertex);
            vertexLineList.Add(guideline);
        }
    }

    Vector3[] SetVertices(int vertexCount, Vector3 normal, float angleOffset, float? noiseScale)
    {
        Vector3 vector2 = normal + new Vector3(1, 1, 1);
        Vector3 planeVector = Vector3.Cross(normal, vector2).normalized;

        for (int i = 0; i < vertexCount; i++)
        {

            float noise = getNoise(i, noiseScale);

            float tunnelRadiusAtPoint = radius + noise;
            float angle = Mathf.Lerp(0, Consts.FullRevolution, i / (float)vertexCount) + angleOffset;

            Quaternion rotation = Quaternion.AngleAxis(angle, normal);
            // // Debug.Log("plane vector " + planeVector);
            Vector3 point = this.center + rotation * (planeVector * tunnelRadiusAtPoint);
            // // Debug.Log("point " + point + " at angle " + angle);
            vertices[i] = point;
        }

        return vertices;
    }

    float getNoise(int index, float? noiseScale)
    {
        return noiseScale.HasValue ? Mathf.PerlinNoise(0, index * (float)noiseScale) * 2 - 1 : defaultNoise;
    }
}

