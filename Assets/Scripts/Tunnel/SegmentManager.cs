using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using static UnityEngine.Rendering.HableCurve;

public class SegmentManager : Singleton<SegmentManager>
{
    public float MinDistFromCap;
    public float MinDistFromCenterLine;

    public const float SameDirAngleMargin = 30; // the margin of error for two gameobjects to be considered facing the same direction

    public static event Action<Transform, Segment> OnEnterNewSegment;
    public Dictionary<string, Segment> SegmentGoDict; // <tunnel name, Segment>

    private void Awake()
    {
        SegmentGoDict = new Dictionary<string, Segment>();
    }

    private void Start()
    {
        float edgeDist = 5; // the distance from the edge of tunnel serves as a stopping point

        MinDistFromCenterLine = TunnelManager.tunnelRadius / 2; // todo: tinker with this to find what number works with creating newly intersected tunnels
        MinDistFromCap = edgeDist;
    }

    public Segment UpdateSegmentFromTransform(Transform transform)
    {
        Segment curSegment = AgentManager.Instance.GetSegment(transform);        
        Segment UpdatedSegment = null;

        if (!curSegment.ContainsTransform(transform)) // another tunnel that is closer than the current tunnel
        {
            UpdatedSegment = GetEnclosingSegment(curSegment, transform);
            if (UpdatedSegment != null)
            {
                Debug.Log("Player has moved to the new segment " + UpdatedSegment.tunnel.name);
                OnEnterNewSegment?.Invoke(transform, UpdatedSegment);
            }
            else
            {
                Debug.LogWarning("Player has not been detected in current segment or any adjoining segments. Default to current segment " + curSegment.tunnel.name);
            }
        }

        return UpdatedSegment;
    }

    //public Segment GetSegmentFromTransform(Transform transform)
    //{
    //    GameObject tunnelGo = TunnelManager.Instance.GetGameObjectTunnel(transform);

    //    return GetSegmentFromObject(tunnelGo);
    //}

    public Segment GetSegmentFromObject(GameObject tunnel)
    {
        if (tunnel == null)
        {
            return null;
        }
        else if (SegmentGoDict.ContainsKey(tunnel.name))
        {
            return SegmentGoDict[tunnel.name];
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
        Segment segment = AgentManager.Instance.GetSegment(transform);

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

        Vector3 intersectingEndRing = intersectingSegment.endRingCenter;
        Vector3 intersectingStartRing = intersectingSegment.startRingCenter;

        // end ring center and start ring center have same direction wrt intersected center line
        Vector3 closestPointToEndRing = intersectedSegment.GetClosestPointToCenterline(intersectingEndRing); // use the guideline to find closest point
        Vector3 closestPointToStartRing = intersectedSegment.GetClosestPointToCenterline(intersectingStartRing); // use the guideline to find closest point

        bool isEndRingCloser = Vector3.Distance(intersectingEndRing, closestPointToEndRing) < Vector3.Distance(intersectingStartRing, closestPointToStartRing);
        Guideline intersectingGuideline = isEndRingCloser ? new Guideline(closestPointToEndRing, intersectingEndRing) : new Guideline(closestPointToStartRing, intersectingStartRing);

        Debug.DrawRay(intersectingGuideline.start, intersectingGuideline.end - intersectingGuideline.start, Color.blue, 100);
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

    public void UpdateConnectingSegmentGuidelines(GameObject curTunnel, List<GameObject> nextTunnels)
    {
        nextTunnels.ForEach((nextTunnel) =>
        {
            AddIntersectingLine(curTunnel, nextTunnel);
        });
    }

    public Segment AddTunnelSegment(SegmentGo segmentGo, List<GameObject> nextTunnels, Ring ring, Ring prevRing)
    {
        Segment segment = new Segment(segmentGo, ring, prevRing);
        GameObject tunnel = segment.tunnel;

        if (nextTunnels.Count > 0)
        {
            SetIntersectedSegments(segment, nextTunnels);
        }

        SegmentGoDict.Add(tunnel.name, segment);

        UpdateConnectingSegmentGuidelines(tunnel, nextTunnels);
        return segment;
    }

    /// <summary>
    /// On intersection, intersecting segments will add each other as next tunnels. Creating bidirectional path in tunnel
    /// </summary>
    /// <param name="intersectingSegment">segment that initiated intersection</param>
    /// <param name="nextTunnels">tunnels that were intersected</param>
    public void SetIntersectedSegments(Segment intersectingSegment, List<GameObject> nextTunnels)
    {
        intersectingSegment.setNextTunnels(nextTunnels);

        nextTunnels.ForEach((tunnel) =>
        {
            Segment nextSegment = GetSegmentFromObject(tunnel);
            nextSegment.setNextTunnel(intersectingSegment.tunnel);
        });
    }
}

