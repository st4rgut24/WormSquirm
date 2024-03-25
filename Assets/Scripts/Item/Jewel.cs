using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using static UnityEngine.Rendering.HableCurve;

public class Jewel : Equipment
{
    public static event Action<Jewel, Segment> CollectJewelEvent;
    public static event Action<Jewel> RepositionJewelEvent;

    public enum Type
    {
        Aquamarine,
        Emerald,
        GoldenStar,
        MoonStone,
        Obsidian,
        PinkSaphire,
        Ruby
    }

    public Type type;

    protected override void OnEnable()
    {
        base.OnEnable();

        TunnelCreatorManager.OnAddCreatedTunnel += OnAddCreatedTunnel;
        TunnelIntersectorManager.OnAddIntersectedTunnelSuccess += OnAddIntersectedTunnel;
    }

    public override void Collect(Segment segment)
    {
        base.Collect(segment);
        CollectJewelEvent?.Invoke(this, segment);
    }

    private bool isCollectible(SegmentGo segmentGo, Transform collectorTransform)
    {
        // check if the jewel's segment matches the collector's segment as well
        if (collectorTransform.CompareTag(Consts.MainPlayerTag))
        {
            GameObject tunnel = segmentGo.getTunnel();

            return Vector3.Distance(transform.position, collectorTransform.position) <= Consts.DistFromNewTunnelEnd;
        }
        else
        {
            return false;
        }
    }

    // TODO: There is a problem, this can return true for multiple jewels when a single segment is created, causing
    // both jewels to be positioned in the center of the segment, instead of the closest one only. This can be solved
    // by making sure gates (and as an effect, keys) are spaced more than one segment apart, which shouldn't be a problem
    // asides from testing
    private void CenterJewelInRange(Transform playerTransform, SegmentGo segmentGo)
    {
        if (!IsCollected && isCollectible(segmentGo, playerTransform))
        {
            Segment segment = SegmentManager.Instance.GetSegmentFromObject(segmentGo.getTunnel());
            Vector3 segmentCenter = segment.GetCenterLineCenter();

            transform.position = segmentCenter;
            RepositionJewelEvent?.Invoke(this);
        }
    }

    void OnAddIntersectedTunnel(Transform playerTransform, SegmentGo segment, GameObject prevTunnel, List<GameObject> startIntersectedTunnels, List<GameObject> endIntersectedTunnels)
    {
        CenterJewelInRange(playerTransform, segment);
    }

    void OnAddCreatedTunnel(Transform playerTransform, SegmentGo segment, GameObject prevTunnel)
    {
        CenterJewelInRange(playerTransform, segment);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        TunnelCreatorManager.OnAddCreatedTunnel -= OnAddCreatedTunnel;
        TunnelIntersectorManager.OnAddIntersectedTunnelSuccess -= OnAddIntersectedTunnel;
    }
}

