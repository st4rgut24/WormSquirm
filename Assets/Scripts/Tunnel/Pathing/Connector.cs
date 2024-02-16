using System;
using System.Collections.Generic;
using System.IO;
using static UnityEngine.Rendering.HableCurve;

public class Connector
{
    Segment segment1;
    Segment segment2;

    Dictionary<Segment, List<Waypoint>> connectorDict;

    public Connector(Segment segment1, Segment segment2)
    {
        connectorDict = new Dictionary<Segment, List<Waypoint>>();

        this.segment1 = segment1;
        this.segment2 = segment2;

        // segment 1 To segment 2 paths
        connectorDict[segment1] = new List<Waypoint>(); // list of points to get from segment 1 to segment 2
        // segment 2 To segment 1 paths
        connectorDict[segment2] = new List<Waypoint>(); // list of points to get from segment 2 to segment 1
    }

    /// <summary>
    /// Get the other segment of the pair
    /// </summary>
    /// <param name="segment">one segment</param>
    public Segment GetConnectedSegment(Segment segment)
    {
        if (segment.tunnel.name != segment1.tunnel.name && segment.tunnel.name != segment2.tunnel.name)
        {
            throw new Exception("This segment does not belong to the connected pair");
        }
        else if (segment.tunnel.name == segment1.tunnel.name)
        {
            return segment2;
        }
        else
        {
            return segment1;
        }
    }

    public void AddPath(Segment segment, List<Waypoint> path)
    {
        connectorDict[segment] = path;
    }

    public List<Waypoint> GetConnectingPath(Segment segment)
    {
        return connectorDict[segment];
    }

    /// <summary>
    /// Set a continous path between two segments
    /// The start of the segment is the point of connection
    /// </summary>
    /// <param name="connectingSegment">segment whose start is the adjacent segment's end</param>
    public void SetContinuousPath(Segment connectingSegment, Segment connectedSegment)
    {
        Guideline connectingCenterline = connectingSegment.centerLine;
        Guideline connectedCenterline = connectedSegment.centerLine;

        List<Waypoint> connectingCenterPath = new List<Waypoint>() { new Waypoint(connectingCenterline.start, connectingSegment) };
        List<Waypoint> connectedCenterPath = new List<Waypoint>() { new Waypoint(connectedCenterline.end, connectingSegment) };

        AddPath(connectingSegment, connectingCenterPath);
        AddPath(connectedSegment, connectedCenterPath);
    }

    /// <summary>
    /// Add paths reflecting connection via intersection
    /// </summary>
    /// <param name="intersectedSegment">The segment that gets a new intersecting guideline</param>
    /// <param name="intersectingSegment">The segment that initiates the intersection</param>
    /// <param name="intersectingLine">the line that intersects one segment</param>
    public void SetIntersectingPath(Segment intersectedSegment, Segment intersectingSegment, Guideline intersectingLine)
    {
        Guideline intersectedGuideline = intersectingSegment.centerLine;

        // one paths from intersected segment to other segment
        List<Waypoint> intersectedPath = new List<Waypoint>() { new Waypoint(intersectingLine.start, intersectedSegment), new Waypoint(intersectingLine.end, intersectingSegment) };

        // one path from intersecting segment to other segment
        List<Waypoint> intersectingPath = new List<Waypoint>() { new Waypoint(intersectedGuideline.end, intersectedSegment) };

        AddPath(intersectingSegment, intersectingPath);
        AddPath(intersectedSegment, intersectedPath);
    }
}

