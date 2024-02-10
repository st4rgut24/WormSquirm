using System;
using UnityEngine;

public class Ring
{
    const float defaultNoise = 0;

    public Vector3[] vertices;

    Vector3 normal;
    float radius;
    Vector3 center;

    public Ring(float radius, int vertexCount, Vector3 normal, Vector3 center, float? noiseScale)
	{
        this.radius = radius;
        this.vertices = new Vector3[vertexCount];
        this.center = center;
        this.normal = normal;

        Debug.Log("Center is " + center);
        setVertices(vertexCount, normal, noiseScale);
    }

    public Vector3 GetCenter()
    {
        return center;
    }

    public Vector3 GetNormal()
    {
        return normal;
    }

    void setVertices(int vertexCount, Vector3 normal, float? noiseScale)
    {
        Vector3 vector2 = normal + new Vector3(1, 1, 1);
        Vector3 planeVector = Vector3.Cross(normal, vector2).normalized;

        for (int i = 0; i < vertexCount; i++)
        {

            float noise = getNoise(i, noiseScale);

            float tunnelRadiusAtPoint = radius + noise;
            float angle = Mathf.Lerp(0, 360f, i / (float)vertexCount);

            Quaternion rotation = Quaternion.AngleAxis(angle, normal);
            // Debug.Log("plane vector " + planeVector);
            Vector3 point = this.center + rotation * (planeVector * tunnelRadiusAtPoint);
            // Debug.Log("point " + point + " at angle " + angle);
            vertices[i] = point;
        }
    }

    float getNoise(int index, float? noiseScale)
    {
        return noiseScale.HasValue ? Mathf.PerlinNoise(0, index * (float)noiseScale) * 2 - 1 : defaultNoise;
    }
}

