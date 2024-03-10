using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SegmentUtils
{
    /// <summary>
    /// Get the end of the guideline that connects to a segment
    /// </summary>
    /// <param name="guideline">A line</param>
    /// <param name="segment">A segment</param>
    /// <returns>the end of the line that is closer to a segment's center, which SHOULD be the connecting end</returns>
    public static Vector3 GetConnectingEndFromGuideline(Guideline guideline, Segment segment)
    {
        Vector3 segmentCenter = segment.GetCenterLineCenter();
        float distToStart = Vector3.Distance(guideline.start, segmentCenter);
        float distToEnd = Vector3.Distance(guideline.end, segmentCenter);

        return distToStart < distToEnd ? guideline.start : guideline.end;
    }

    /// <summary>
    /// Gets the next downhill tunnel with the steepest descent
    /// </summary>
    /// <param name="segment">current segment</param>
    /// <returns>next downhill segment or null if it doesnt exist</returns>
    public static Segment GetNextTunnelWithSteepestDescent(Segment segment)
    {
        List<GameObject> nextTunnels = segment.getNextTunnels();

        // segment that has the deepest descent
        Segment steepestDownhillSegment = null;
        float maxElevation = segment.GetElevation();

        nextTunnels.ForEach((nextTunnel) =>
        {
            Segment nextSegment = SegmentManager.Instance.GetSegmentFromObject(nextTunnel);
            float elevation = nextSegment.GetElevation();

            if (elevation < maxElevation)
            {
                maxElevation = elevation;
                steepestDownhillSegment = nextSegment;
            }
        });

        return steepestDownhillSegment;
    }

    /// <summary>
    /// Is next tunnel downhill of segment
    /// </summary>
    /// <param name="segment"></param>
    /// <param name="nextTunnel"></param>
    /// <returns>true if next tunnel is downhill of segment</returns>
    public static bool IsSegmentsDownhill(Segment segment, GameObject nextTunnel)
    {
        Segment nextSegment = SegmentManager.Instance.GetSegmentFromObject(nextTunnel);

        if (segment != null && nextSegment != null)
        {
            float segmentY = segment.GetElevation();
            float nextSegmentY = nextSegment.GetElevation();

            return segmentY > nextSegmentY;
        }
        else
        {
            return false;
        }
    }
}

