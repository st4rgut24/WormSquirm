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
        Segment UpdatedSegment = null;

        if (!curSegment.ContainsTransform(transform)) // another tunnel that is closer than the current tunnel
        {
            UpdatedSegment = GetEnclosingSegment(curSegment, transform);
            Debug.Log("Player has moved to the new segment " + UpdatedSegment.tunnel.name);
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
    /// Get the segment that encloses a transform
    /// </summary>
    /// <param name="transform">enclosed transform</param>
    /// <returns>enclosing segment</returns>
    public Segment GetEnclosingSegment(Segment curSegment, Transform transform)
    {
        Segment enclosingSegment = null;
        List<GameObject> segmentObjects = TunnelUtils.GetAdjoiningTunnels(curSegment);

        segmentObjects.ForEach((segmentObj) =>
        {
            Segment segment = GetSegmentFromObject(segmentObj);

            if (segment.ContainsTransform(transform))
            {
                if (enclosingSegment == null)
                {
                    enclosingSegment = segment;
                }
                else
                {
                    throw new Exception("Multiple enclosing segments found that contain transform at position " + transform.position);
                }
            }
        });

        return enclosingSegment;
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
    /// Create a line inside the intersected object that connects to the initiator
    /// </summary>
    /// <param name="intersectorInitiator">the object that caused the intersection</param>
    /// <param name="intersectedObject">the object that was intersected</param>
    public void AddIntersectingLineToSegment(GameObject intersectorInitiator, GameObject intersectedObject)
    {
        Segment intersectingSegment = GetSegmentFromObject(intersectorInitiator);
        Segment intersectedSegment = GetSegmentFromObject(intersectedObject);

        Vector3 lineEnd = intersectingSegment.endRingCenter; // the end of the intersecting segment is what intersects the segment
        Vector3 lineStart = intersectedSegment.GetClosestPointToCenterline(lineEnd); // use the guideline to find closest point
        Guideline intersectingGuideline = new Guideline(lineStart, lineEnd);

        Debug.DrawRay(lineStart, lineEnd - lineStart, Color.green, 100);
        intersectedSegment.AddGuideline(intersectingGuideline);
    }

    /// <summary>
    /// Add guidelines between intersecting tunnels
    /// </summary>
    public void AddIntersectingLine(GameObject tunnel, GameObject otherTunnel)
    {
        if (tunnel == null || otherTunnel == null)
        {
            return;
        }
        if (TunnelManager.Instance.IsIntersectingInitiator(tunnel, otherTunnel))
        {
            AddIntersectingLineToSegment(tunnel, otherTunnel);
        }
        else if (TunnelManager.Instance.IsIntersectingInitiator(otherTunnel, tunnel))
        {
            AddIntersectingLineToSegment(otherTunnel, tunnel);
        }
    }

    public void UpdateConnectingSegmentGuidelines(GameObject curTunnel, GameObject prevTunnel, List<GameObject> nextTunnels)
    {
        // from prev to cur
        AddIntersectingLine(curTunnel, prevTunnel);

        // from cur to each next tunnel
        nextTunnels.ForEach((nextTunnel) =>
        {
            AddIntersectingLine(curTunnel, nextTunnel);
        });
    }

    public Segment AddTunnelSegment(GameObject tunnel, GameObject prevTunnel, List<GameObject> nextTunnels, Ring ring, Ring prevRing)
    {
        Segment segment = new Segment(tunnel, prevTunnel, ring, prevRing);

        if (prevTunnel != null)
        {
            Segment prevSegment = SegmentDict[prevTunnel.name];
            prevSegment.setNextTunnel(tunnel);
        }       

        segment.setNextTunnels(nextTunnels);
        SegmentDict.Add(tunnel.name, segment);

        UpdateConnectingSegmentGuidelines(tunnel, prevTunnel, nextTunnels);
        return segment;
    }
}

