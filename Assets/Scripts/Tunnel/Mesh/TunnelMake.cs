using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UIElements;
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
    /// <param name="playerTransform">The key used to look up previous segments</param>
    /// <param name="heading">directional info</param>
    /// <param name="isClosed">whether the tunnel is closed/capped</param>
    /// <returns></returns>
    public SegmentGo GrowTunnel(Transform playerTransform, Heading heading, bool isClosed, Ring prevRing)
    {
        Vector3 direction = heading.forward;
        Vector3 position = heading.position;

        return CreateSegment(direction, position, playerTransform, isClosed, prevRing);

    }

    private SegmentGo CreateSegment(Vector3 direction, Vector3 position, Transform playerTransform, bool isClosed, Ring prevRing)
    {
        Ring endRing = RingManager.Instance.Create(direction, position);
        //Ring prevRing = RingManager.Instance.Get(playerTransform);

        OptionalMeshProps meshProps = new OptionalMeshProps(playerTransform, prevRing, _props);

        GameObject tunnelObject = MeshObjectFactory.Get(MeshType.Tunnel, TunnelSegment, endRing, meshProps);

        tunnelObject.name = "Tunnel " + tunnelCounter;
        tunnelCounter++;

        GameObject capObject = GetEndCap(endRing, Cap, playerTransform, isClosed);

        Corridor corridor = new Corridor(tunnelObject, endRing, prevRing);
        return new SegmentGo(capObject, corridor);
    }

    /// <summary>
    /// If no tunnel exists, need to initialize the start ring of the tunnel
    /// </summary>
    /// <param name="direction">tunnel direction</param>
    /// <param name="playerTransform">transform of player</param>
    /// <returns>ring defining start of tunnel</returns>
    private Ring InitTunnel(Vector3 direction, Transform playerTransform)
    {
        // the first ring should use player position to connect to the next ring with offset
        Ring ring = RingManager.Instance.Create(direction, playerTransform.position);
        RingManager.Instance.Add(playerTransform, ring); // update normal vector

        return ring;
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
            return MeshObjectFactory.Get(MeshType.EndCap, prefab, ring, new OptionalMeshProps());
        }
        else
        {
            return null;
        }
    }
}
