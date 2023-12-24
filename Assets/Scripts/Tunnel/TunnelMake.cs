using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TunnelMake : MonoBehaviour
{
    public GameObject TunnelSegment;

    public int tunnelSegments = 3;
    public float segmentSpacing = 1.0f;
    public float tunnelRadius = 2.0f;
    public float noiseScale = 0.1f;

    float prevHeight = 0;
    int growCounter = 0;

    bool startFlag = true;
    Vector3 oldLocation;

    private Vector3[] vertices;

    List<Ring> rings;

    private void OnEnable()
    {
        Player.OnMove += GrowTunnel;
    }

    void Start()
    {
        rings = new List<Ring>();
    }

    public void GrowTunnel(Vector3 position)
    {
        if (startFlag)
        {
            startFlag = false;
        }
        else if (position.y - oldLocation.y > 0) // if we are moving in position upwards direction
        {
            Debug.Log("Grow Tunnel");
            GenerateTunnel(oldLocation.y, position.y);
        }
        oldLocation = position;
    }

    void GenerateTunnel(float startHeight, float endHeight)
    {
        GameObject segment = Instantiate(TunnelSegment);
        MeshFilter meshFilter = segment.GetComponent<MeshFilter>();

        Mesh tunnelMesh = new Mesh();
        meshFilter.mesh = tunnelMesh;

        vertices = new Vector3[tunnelSegments * 2];

        for (int i = 0; i < tunnelSegments; i++)
        {
            float t = i * segmentSpacing;
            //float noise = Mathf.PerlinNoise(0, t * noiseScale) * 2 - 1; // Adjust noise scale as needed

            //float tunnelRadiusAtPoint = tunnelRadius + noise;
            float angle = Mathf.Lerp(0, 2 * Mathf.PI, i / (float)tunnelSegments);

            float x = Mathf.Cos(angle) * tunnelRadius; // tunnelRadiusAtPoint;
            float z = Mathf.Sin(angle) * tunnelRadius; // tunnelRadiusAtPoint;

            vertices[i] = new Vector3(x, startHeight, z);
            vertices[i + tunnelSegments] = new Vector3(x, endHeight, z); // Adjust tunnel height as needed
        }

        tunnelMesh.vertices = vertices;

        int[] triangles = new int[6 * tunnelSegments];

        for (int i = 0, ti = 0; i < tunnelSegments; i++, ti += 6)
        {
            // last tunnel segment connects the first triangle to the last triangle
            if (i == tunnelSegments - 1)
            {
                triangles[ti] = i;
                triangles[ti + 1] = 0;
                triangles[ti + 2] = tunnelSegments;

                triangles[ti + 3] = tunnelSegments;
                triangles[ti + 4] = i + tunnelSegments;
                triangles[ti + 5] = i;
            }
            else
            {
                triangles[ti] = i;
                triangles[ti + 1] = i + 1;
                triangles[ti + 2] = i + tunnelSegments;

                triangles[ti + 3] = i + 1;
                triangles[ti + 4] = i + tunnelSegments + 1;
                triangles[ti + 5] = i + tunnelSegments;
            }
        }

        tunnelMesh.triangles = triangles;
    }

    private void OnDisable()
    {
        Player.OnMove -= GrowTunnel;
    }
}
