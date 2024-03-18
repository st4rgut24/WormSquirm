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

        OptionalMeshProps meshProps = new OptionalMeshProps(playerTransform, prevRing, _props);
        return CreateSegment(direction, position, meshProps, prevRing);
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

        OptionalMeshProps meshProps = new OptionalMeshProps(playerTransform, offsetPrevRing, _props);
        // the un-offset ring is used to create the segment, to create the true center line which begins where the previous tunnel ends,
        // otherwise the start of this tunnel would overlap with the previous tunnel, leading to ambiguity when determining which tunnel
        // a player belonged to
        return CreateSegment(tunnelDir, position, meshProps, prevRing);
    }

    private SegmentGo CreateSegment(Vector3 direction, Vector3 position, OptionalMeshProps meshProps, Ring prevRing)
    {
        Ring endRing = RingFactory.Create(direction, position);


        GameObject tunnelObject = MeshObjectFactory.Get(MeshType.Tunnel, TunnelSegment, endRing, meshProps);
        tunnelObject.transform.parent = TunnelContainer;
        tunnelObject.tag = Consts.TunnelTag;

        tunnelObject.name = "Tunnel " + tunnelCounter;
        tunnelCounter++;

        return new SegmentGo(tunnelObject, Cap, endRing, prevRing, meshProps.prevRing);
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
