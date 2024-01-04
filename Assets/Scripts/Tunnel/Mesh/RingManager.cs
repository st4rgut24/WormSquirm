using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Maintains a mapping of Transforms to Rings
/// </summary>
public class RingManager: Singleton<RingManager>
{
    TunnelProps _props;

    private Dictionary<Transform, Ring> RingDict; // maps Player Transform to previous rings

    public RingManager()
	{
        RingDict = new Dictionary<Transform, Ring>();
	}

    private void Awake()
    {
        _props = TunnelManager.Instance.defaultProps;
    }

    public bool ContainsRing(Transform transform)
    {
        return RingDict.ContainsKey(transform);
    }

    public void Add(Transform transform, Ring ring)
    {
        RingDict.Add(transform, ring);
    }

    public void UpdateEntry(Transform transform, Ring ring)
    {
        RingDict[transform] = ring;
    }

    public void Remove(Transform transform)
    {
        RingDict.Remove(transform);
    }

    public Ring Get(Transform transform)
    {
        return RingDict[transform];
    }

    public Ring Create(Vector3 direction, Vector3 position)
    {
        return new Ring(_props.TunnelRadius, _props.TunnelSegments, direction, position, _props.NoiseScale);
    }

    public Ring Create(float radius, int segments, Vector3 direction, Vector3 position, float? noiseScale)
    {
        return new Ring(radius, segments, direction, position, noiseScale);
    }
}

