using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Maintains a mapping of Transforms to Rings
/// </summary>
public class RingFactory: MonoBehaviour
{
    static TunnelProps _props;

    private void Awake()
    {
        _props = TunnelManager.Instance.defaultProps;
    }

    public static Ring Create(Vector3 direction, Vector3 position)
    {
        return new Ring(_props.TunnelRadius, _props.TunnelSides, direction, position, _props.NoiseScale);
    }

    public static Ring Create(float radius, int segments, Vector3 direction, Vector3 position, float? noiseScale)
    {
        return new Ring(radius, segments, direction, position, noiseScale);
    }

    /// <summary>
    /// Create a list of rings between startRing and endRing
    /// </summary>
    /// <param name="startRing">ring at beginning of tunnel</param>
    /// <param name="endRing">ring at end of tunnel</param>
    /// <returns>list of rings</returns>
    public static List<Ring> CreateRings(Ring startRing, Ring endRing)
    {
        List<Ring> rings = new List<Ring>() { startRing };

        Vector3 startCenter = startRing.GetCenter();
        Vector3 dir = (endRing.GetCenter() - startRing.GetCenter()).normalized;

        float dist = Vector3.Distance(endRing.GetCenter(), startCenter);

        float ringCount = (int) (dist / _props.SideLength); // dist between each ring should equal length of side
        float sliceCount = ringCount + 1;
        float sliceLen = dist / sliceCount; // equal distance between slices, approximately equal to tunnel side length

        for (int i=0; i < ringCount; i++)
        {
            float distFromStart = sliceLen * (i + 1);
            Vector3 center = startCenter + distFromStart * dir;
            Ring ring = RingFactory.Create(dir, center);
            rings.Add(ring);
        }

        rings.Add(endRing);

        return rings;
    }
}

