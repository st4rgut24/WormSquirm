using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

public class TunnelMake: MonoBehaviour
{
    // Rectangle GameObject to represent the UV mapping area
    public GameObject rectangleGameObject;

    public GameObject TunnelSegment;
    public GameObject Cap;

    TunnelProps _props;

    float prevHeight = 0;

    int tunnelCounter = 0;

    Vector3 startPosition = new Vector3(0, 0, 0);

    private Vector3[] vertices;


    private void Awake()
    {
    }

    private void Start()
    {
        _props = TunnelManager.Instance.defaultProps;
    }

    /// <summary>
    /// Create a Tunnel
    /// </summary>
    /// <param name="transform">The key used to look up previous segments</param>
    /// <param name="heading">directional info</param>
    /// <param name="isClosed">whether the tunnel is closed</param>
    /// <returns></returns>
    public SegmentGo GrowTunnel(Transform transform, Heading heading, bool isClosed)
    {
        Vector3 direction = heading.forward;
        Vector3 position = heading.position;

        if (!RingManager.Instance.ContainsRing(transform)) // initialize start of tunnel
        {
            Ring ring = RingManager.Instance.Create(direction, position);
            RingManager.Instance.Add(transform, ring); // update normal vector

            return null;
        }
        else
        {
            Ring ring = RingManager.Instance.Create(direction, position);

            GameObject tunnelObject = MeshObjectFactory.Get(MeshType.Tunnel, TunnelSegment, transform, ring, _props);

            tunnelObject.name = "Tunnel " + tunnelCounter;
            tunnelCounter++;

            GameObject capObject = GetEndCap(ring, Cap, transform, isClosed);

            SegmentGo segmentGo = new SegmentGo(tunnelObject, capObject);
            TunnelManager.Instance.AddGameObjectSegment(transform, segmentGo.segment);

            return segmentGo;
        }
    }

    /// <summary>
    /// Generate a cap for the end of the tunnel
    /// </summary>
    /// <param name="ring">the vertices of the cap</param>
    /// <param name="isClosed">Whether the tunnel is closed off or not</param>
    /// <returns>An Cap GameObject</returns>
    GameObject GetEndCap(Ring ring, GameObject prefab, Transform transform, bool isClosed)
    {
        if (isClosed)
        {
            return MeshObjectFactory.Get(MeshType.EndCap, prefab, transform, ring, _props);
        }
        else
        {
            return null;
        }
    }
}
