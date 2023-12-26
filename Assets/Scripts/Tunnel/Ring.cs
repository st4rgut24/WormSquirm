using System;
using UnityEngine;

public class Ring
{
    public Vector3[] vertices;

    float radius;
    Vector3 center;

    public Ring(float radius, int vertexCount, float vertexSpacing, Vector3 normal, Vector3 center)
	{
        this.radius = radius;
        this.vertices = new Vector3[vertexCount];
        this.center = center;

        setVertices(vertexCount, vertexSpacing, Vector3.forward);
    }

    void setVertices(int vertexCount, float vertexSpacing, Vector3 normal)
    {

        for (int i = 0; i < vertexCount; i++)
        {
            float t = i * vertexSpacing;
            //float noise = Mathf.PerlinNoise(0, t * noiseScale) * 2 - 1; // Adjust noise scale as needed

            //float tunnelRadiusAtPoint = tunnelRadius + noise;
            float angle = Mathf.Lerp(0, 360f, i / (float)vertexCount);

            Quaternion rotation = Quaternion.AngleAxis(angle, normal);
            Vector3 vector2 = normal + new Vector3(1, 1, 1) ;
            Vector3 planeVector = Vector3.Cross(normal, vector2).normalized;
            Vector3 point = this.center + rotation * (planeVector * radius);
            Debug.Log(point);
            vertices[i] = point;
        }
    }
}

