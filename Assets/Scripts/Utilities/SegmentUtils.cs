using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SegmentUtils
{
    /// <summary>
    /// Gets a point perpendicular to the segment at a certain distance in a random direction
    /// </summary>
    /// <param name="segment">segment to find distance from</param>
    /// <param name="up">the normalized up direction used to get point's position</param>
    /// <param name="distance">distance</param>
    /// <param name="angle">Angle to rotate a point about segment forward vector</param>
    /// <returns>a random point in space</returns>
    public static Vector3 GetPerpendicularPoint(Segment segment, int distance, float angle)
    {
        Ring endRing = segment.GetEndRing();

        Vector3 point1 = endRing.center;

        //Vector3 levelPoint = endRing.vertices[0]; // point should be on same plane as the segment, 0 degrees
        Quaternion rotationAboutAxis = Quaternion.AngleAxis(-angle, segment.forward);

        Vector3 rotatedDir = (rotationAboutAxis * segment.right).normalized;
        Vector3 point = segment.GetCenterLineCenter() + rotatedDir * distance;

        return point;
    }

    /// <summary>
    /// Gets a point collinear to the segment provided a direction from the segment's center
    /// </summary>
    /// <param name="segment">segment to find distance from</param>
    /// <param name="distance">distance</param>
    /// <returns>a point along the same line the segment is going in</returns>
    public static Vector3 GetCollinearPoint(Segment segment, int distance, Vector3 direction)
    {
        return segment.GetCenterLineCenter() + direction * distance;
    }

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

    /// <summary>
    /// Gets the first consecutive segment
    /// </summary>
    /// <param name="segment"></param>
    /// <returns></returns>
    public static Segment GetNextSegment(Segment segment)
    {
        List<GameObject> nextSegments = segment.getNextTunnels();

        if (nextSegments.Count == 0)
        {
            return null;
        }
        else
        {
            GameObject nextSegmentGo = nextSegments[0];
            Segment nextSegment = SegmentManager.Instance.GetSegmentFromObject(nextSegmentGo);
            return nextSegment;
        }
    }
}

