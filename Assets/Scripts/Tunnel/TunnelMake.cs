using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class TunnelMake : MonoBehaviour
{
    public GameObject TunnelSegment;

    public int tunnelSegments = 10;
    public float segmentSpacing = 1.0f;
    public float tunnelRadius = 2.0f;
    public float noiseScale = 0.1f;

    float prevHeight = 0;
    int growCounter = 0;

    Vector3 startPosition = new Vector3(0, 0, 0);


    private Vector3[] vertices;

    Ring prevRing = null;
    Vector3 prevPos = new Vector3(-1, -1, -1);

    private void OnEnable()
    {
        Player.OnMove += GrowTunnel;
    }

    private void Awake()
    {
        
    }

    public void GrowTunnel(Transform transform)
    {
        Vector3 position = transform.position;
        Vector3 direction = transform.forward;

        if (prevRing == null) // initialize start of tunnel
        {
            prevRing = RingFactory.get(tunnelRadius, segmentSpacing, tunnelSegments, direction, position); // update normal vector
        }
        else
        {
            GenerateTunnel(position, direction);
        }
    }

    void GenerateTunnel(Vector3 position, Vector3 direction)
    {
        GameObject segment = Instantiate(TunnelSegment);
        MeshFilter meshFilter = segment.GetComponent<MeshFilter>();

        Mesh tunnelMesh = new Mesh();
        meshFilter.mesh = tunnelMesh;

        Ring ring = RingFactory.get(tunnelRadius, segmentSpacing, tunnelSegments, direction, position);

        Vector3[] vertices = prevRing.vertices.Concat(ring.vertices).ToArray();

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

        prevRing = ring;
        prevPos = position;
    }

    private void OnDisable()
    {
        Player.OnMove -= GrowTunnel;
    }
}
