using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.HableCurve;

public class TunnelMake: MonoBehaviour
{
    public Transform TunnelContainer;

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
    /// <returns></returns>
    public SegmentGo GrowTunnel(Transform playerTransform, Heading heading, Ring prevRing)
    {
        Vector3 direction = heading.forward;
        Vector3 position = heading.position;

        return CreateSegment(direction, position, playerTransform, prevRing);
    }

    /// <summary>
    /// Creates an Offset Tunnel, extended on both sides
    /// </summary>
    /// <param name="offset">amount to extend the tunnel on either end</param>
    /// <returns>offset tunnel, typically used for overlapping segments</returns>
    public SegmentGo GrowExtendedTunnel(Transform playerTransform, Heading heading, Ring prevRing, float offset)
    {
        Vector3 tunnelDir = heading.forward;

        Vector3 prevRingDir = -tunnelDir;
        Vector3 prevRingCenter = prevRing.center + prevRingDir * offset;
        Ring offsetPrevRing = RingFactory.Create(tunnelDir, prevRingCenter);

        Vector3 position = heading.position + tunnelDir * offset;

        return CreateSegment(tunnelDir, position, playerTransform, offsetPrevRing); // TODO: not working properly on game start intersection
    }

    private SegmentGo CreateSegment(Vector3 direction, Vector3 position, Transform playerTransform, Ring prevRing)
    {
        Ring endRing = RingFactory.Create(direction, position);
        //Ring prevRing = RingManager.Instance.Get(playerTransform);

        OptionalMeshProps meshProps = new OptionalMeshProps(playerTransform, prevRing, _props);

        GameObject tunnelObject = MeshObjectFactory.Get(MeshType.Tunnel, TunnelSegment, endRing, meshProps);
        tunnelObject.transform.parent = TunnelContainer;
        tunnelObject.tag = Consts.TunnelTag;

        tunnelObject.name = "Tunnel " + tunnelCounter;
        tunnelCounter++;

        return new SegmentGo(tunnelObject, Cap, endRing, prevRing);
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
        Ring ring = RingFactory.Create(direction, playerTransform.position);

        return ring;
    }
}
