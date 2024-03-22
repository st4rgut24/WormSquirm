using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using static UnityEngine.Rendering.HableCurve;

public class Jewel : Valuable
{
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

    private bool isCollectible(SegmentGo segmentGo, Transform collectorTransform)
    {
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

    private void CenterJewelInRange(Transform playerTransform, SegmentGo segmentGo)
    {
        if (isCollectible(segmentGo, playerTransform))
        {
            Segment segment = SegmentManager.Instance.GetSegmentFromObject(segmentGo.getTunnel());
            Vector3 segmentCenter = segment.GetCenterLineCenter();

            transform.position = segmentCenter;
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

