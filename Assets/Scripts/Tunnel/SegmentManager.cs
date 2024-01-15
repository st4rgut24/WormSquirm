using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SegmentManager : Singleton<SegmentManager>
{
    public Dictionary<string, Segment> SegmentDict; // <tunnel name, Segment)

    private void Awake()
    {
        SegmentDict = new Dictionary<string, Segment>();
    }

    public Segment GetSegmentFromTransform(Transform transform)
    {
        GameObject tunnelGo = TunnelManager.Instance.GetGameObjectTunnel(transform);

        return GetSegmentFromObject(tunnelGo);
    }

    public Segment GetSegmentFromObject(GameObject tunnel)
    {
        if (tunnel == null)
        {
            return null;
        }
        else if (SegmentDict.ContainsKey(tunnel.name))
        {
            return SegmentDict[tunnel.name];
        }
        else
        {
            throw new System.Exception("No Segment matches tunnel named " + tunnel.name);
        }
    }

    public void AddTunnelSegment(GameObject tunnel, GameObject prevTunnel, List<GameObject> nextTunnels, Ring ring, Ring prevRing)
    {
        Segment segment = new Segment(tunnel, prevTunnel, ring, prevRing);

        if (prevTunnel != null)
        {
            Segment prevSegment = SegmentDict[prevTunnel.name];
            prevSegment.setNextTunnel(tunnel);
        }

        segment.setNextTunnels(nextTunnels);
        SegmentDict.Add(tunnel.name, segment);
    }
}

