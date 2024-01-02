using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class TunnelMake: MonoBehaviour
{
    public GameObject TunnelSegment;

    TunnelProps _props;

    float prevHeight = 0;

    int tunnelCounter = 0;

    Vector3 startPosition = new Vector3(0, 0, 0);

    private Vector3[] vertices;

    private Dictionary<Transform, Ring> PrevRingDict; // maps Player Transform to previous rings

    private void Awake()
    {
        PrevRingDict = new Dictionary<Transform, Ring>();
    }

    private void Start()
    {
        setProps(TunnelManager.Instance.defaultProps);
    }

    public void setProps(TunnelProps props)
    {
        _props = props;
    }

    public void clearPrevRingEntry(Transform transform)
    {
        PrevRingDict.Remove(transform);
    }

    public GameObject GrowTunnel(Transform transform)
    {
        Vector3 position = transform.position;
        Vector3 direction = transform.forward;

        GameObject tunnelObject = null;

        if (!PrevRingDict.ContainsKey(transform)) // initialize start of tunnel
        {
            Ring ring = RingFactory.get(_props.TunnelRadius, _props.TunnelSegments, direction, position, _props.NoiseScale);
            PrevRingDict.Add(transform, ring); // update normal vector
        }
        else
        {
            tunnelObject = GenerateTunnelMesh(transform, position, direction);
            tunnelObject.name = "Tunnel " + tunnelCounter;
            tunnelCounter++;
        }

        return tunnelObject;
    }

    /// <summary>
    /// Calculate a Tunnel's Mesh
    /// </summary>
    /// <param name="transform">Transform of the agent that created the mesh</param>
    /// <param name="position">position of the ring</param>
    /// <param name="direction">direction the tunnel is growing</param>
    /// <returns></returns>
    GameObject GenerateTunnelMesh(Transform transform, Vector3 position, Vector3 direction)
    {
        GameObject segment = Instantiate(TunnelSegment);
        MeshFilter meshFilter = segment.GetComponent<MeshFilter>();

        Mesh tunnelMesh = new Mesh();
        meshFilter.mesh = tunnelMesh;

        Ring ring = RingFactory.get(_props.TunnelRadius, _props.TunnelSegments, direction, position, _props.NoiseScale);

        Vector3[] vertices = PrevRingDict[transform].vertices.Concat(ring.vertices).ToArray();

        int tunnelSegments = _props.TunnelSegments;

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

        PrevRingDict[transform] = ring;

        return segment;
    }
}
