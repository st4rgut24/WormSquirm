using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SegmentManager : Singleton<SegmentManager>
{
    public float MinDistFromCap;
    public float MinDistFromCenterLine;

    public const float SameDirAngleMargin = 30; // the margin of error for two gameobjects to be considered facing the same direction

    public static event Action<Transform, Segment> OnNewSegment;
    public Dictionary<string, Segment> SegmentDict; // <tunnel name, Segment)

    private void Awake()
    {
        SegmentDict = new Dictionary<string, Segment>();
    }

    private void Start()
    {
        float edgeDist = 5; // the distance from the edge of tunnel serves as a stopping point

        MinDistFromCenterLine = TunnelManager.tunnelRadius / 2; // todo: tinker with this to find what number works with creating newly intersected tunnels
        MinDistFromCap = edgeDist;
    }   

    public Segment UpdateSegmentFromTransform(Transform transform)
    {
        Segment curSegment = GetSegmentFromTransform(transform);

        List<GameObject> tunnels = new List<GameObject> ( curSegment.getNextTunnels() );
        tunnels.AddRange(curSegment.getPrevTunnels());
        tunnels.Add(curSegment.tunnel);

        GameObject enclosingTunnel = TunnelUtils.getClosestObject(transform.position, tunnels);
        Segment UpdatedSegment = null;

        if (enclosingTunnel != curSegment.tunnel) // another tunnel that is closer than the current tunnel
        {
            UpdatedSegment = GetSegmentFromObject(enclosingTunnel);
            Debug.Log("Player has moved to the new segment " + enclosingTunnel.name);
            OnNewSegment?.Invoke(transform, UpdatedSegment);
        }

        return UpdatedSegment;
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

    /// <summary>
    /// Check whether a tunnel gameobject is linked to another tunnel
    /// </summary>
    /// <param name="tunnel"></param>
    /// <param name="otherTunnel"></param>
    /// <returns></returns>
    public bool IsTunnelsConnected(GameObject tunnel, GameObject otherTunnel)
    {
        if (tunnel == null)
        {
            return false;
        }

        Segment segment = GetSegmentFromObject(tunnel);
        List<GameObject> nextTunnels = segment.getNextTunnels();
        return nextTunnels.Contains(otherTunnel);
    }

    /// <summary>
    /// Check if the player's digging is extending the tunnel
    /// </summary>
    /// <param name="transform">transform of player</param>
    /// <returns>true if digging extends rather than bisects tunnel</returns>
    public bool IsExtendingTunnel(Transform transform)
    {
        Segment segment = GetSegmentFromTransform(transform);

        if (segment == null) // start of game, start of a tunnel
        {
            return true;
        }
        else if (segment.hasEndCap())
        {
            float angle = Vector3.Angle(transform.forward, segment.forward);
            return angle <= SameDirAngleMargin;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Check if the transform is outside segment bounds
    /// </summary>
    /// <param name="transform">transform</param>
    /// <param name="position">position to check for bounds</param>
    /// <returns>true if out of bounds</returns>
    public bool  IsSegmentBoundsExceeded(Transform transform, Vector3 position)
    {
        Segment segment = GetSegmentFromTransform(transform);

        if (segment != null)
        {
            if (segment.hasEndCap()) // check if player is close enough to the end of a tunnel
            {
                float distToEndCap = Vector3.Distance(position, segment.endRingCenter);

                //Debug.Log("Distance to end cap is " + distToEndCap + ". Min dist to end cap is " + MinDistFromCap);
                if (distToEndCap <= MinDistFromCap)
                {
                    return true;
                } 
            }
            float dist = segment.GetClosestDistanceToCenterLine(position);

            //Debug.Log("Distance to center line is " + dist + ". Max dist from center line to be mobile is " + (TunnelManager.tunnelRadius / 2));
            return dist >= MinDistFromCenterLine; // divide by 2 because the moveable area is smaller on bottom plane of the tunnel
        }
        else
        {
            return false;
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

