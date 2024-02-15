using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.HableCurve;


public enum RouteStrat
{
    FollowSegment,
    StraightPath
}

/// <summary>
/// Gets the route for an Agent
/// </summary>
public class RouteFactory
{
    public static int defaultSegmentDist = 12; // number of segments away from destination
    public static int waypointInterval = 1; // how many segments between waypoints

    /// <summary>
    /// Get a Route for a agent to follow
    /// </summary>
    /// <param name="strat">Strategy used for constructing a route</param>
    /// <param name="targetTransform">Destination of the route</param>
    /// <param name="agent">Agent that the route will be assigned to</param>
    /// <returns></returns>
    public static Route Get(RouteStrat strat, Agent agent, Transform targetTransform)
    {
        switch (strat)
        {
            case RouteStrat.FollowSegment:
                return FollowSegments(agent.curSegment, targetTransform, agent.transform);
            case RouteStrat.StraightPath:
                return CreateStraightPath(targetTransform);
            default:
                return FollowSegments(agent.curSegment, targetTransform, agent.transform);
        }
    }

    /// <summary>
    /// Creates a route consisting of a straight path between two points
    /// </summary>
    /// <param name="targetTransform">destination</param>
    /// <returns>a simple route</returns>
    public static Route CreateStraightPath(Transform targetTransform)
    {
        Route route = new Route();
        Transform simpStartBlock = AgentManager.Instance.botManager.SimpStartBlock;

        route.AddWaypoint(new Waypoint(simpStartBlock.position));
        route.AddWaypoint(new Waypoint(targetTransform.position));

        return route;
    }

    /// <summary>
    /// Get the route that traverses segments
    /// </summary>
    /// <param name="segments">The segments that are part of the route</param>
    /// <param name="endWP">The destination waypoint</param>
    /// <returns>the route</returns>
    public static Route GetFollowSegmentsRoute(List<Segment> segments, Waypoint endWP)
    {
        Route route = new Route();

        for (int i=1;i<segments.Count; i++)
        {
            Segment startSegment = segments[i - 1];
            Segment destSegment = segments[i];

            List<Waypoint> path = SegmentManager.Instance.GetConnectedSegmentPath(startSegment, destSegment);
            route.AddWaypoints(path);
        }

        route.AddWaypoint(endWP);
        return route;
    }

    /// <summary>
    /// Create a path when an agent is first assigned a route, when agent is not yet in a segment
    /// </summary>
    /// <param name="targetTransform">the transform of target gameobject</param>
    /// <returns></returns>
    public static List<Segment> StartFollowSegments(Segment goalSegment)
    {
        int dist = 0;
        Segment segment = goalSegment;

        HashSet<GameObject> seenTunnels = new HashSet<GameObject>() { goalSegment.tunnel };
        List<Segment> segments = new List<Segment>();

        // farthest away the start segment can be from the goal segment is length of defaultSegmentDist
        while (dist <= defaultSegmentDist) {
            segments.Add(segment);

            List<GameObject> neighborTunnels = segment.getNextTunnels();

            if (neighborTunnels.Count == 0)
            {
                throw new Exception("Cannot follow segment, because missing previous tunnels");
            }

            GameObject prevTunnel = null;

            for (int i=0;i<neighborTunnels.Count;i++)
            {
                GameObject neighborTunnel = neighborTunnels[i];

                // make sure the path does not go in loops, and the tunnels in the path are currently active
                if (neighborTunnel.activeSelf && !seenTunnels.Contains(neighborTunnel))
                {
                    seenTunnels.Add(neighborTunnel);
                    prevTunnel = neighborTunnel;
                }
            }

            if (prevTunnel == null)
            {
                break;
            }
            else
            {
                segment = SegmentManager.Instance.GetSegmentFromObject(prevTunnel);
                dist++;
            }
        }

        segments.Reverse(); // make the list go from start segment to the goal segment
        return segments;
    }

    /// <summary>
    /// Create a route that reaches target by following existing segments
    /// </summary>
    /// <param name="curSegment">current segment of the agent being assigned a route</param>
    /// <param name="targetTransform">the transform of the target agent</param>
    /// <param name="transform">transform of the agent being assigned route</param>
    /// <returns>a route ending up in the segment that target gameobject is in</returns>
    public static Route FollowSegments(Segment curSegment, Transform targetTransform, Transform transform)
    {
        Segment goalSegment = AgentManager.Instance.GetSegment(targetTransform);
        bool isInSegment = curSegment != null;

        List<Segment> segments = isInSegment ?
            SearchUtils.dfsConnectSegments(goalSegment, curSegment) :
            StartFollowSegments(goalSegment);


        //Waypoint startWaypoint = isInSegment ? new Waypoint(transform.position, segments[0]) : new Waypoint(segments[0].getCenter(), segments[0]);
        // TODO: final waypoint will be a point along an existing guideline. Don't go to the target's position
        Waypoint endWaypoint = new Waypoint(targetTransform.position, segments[segments.Count - 1]);

        // convert a list of segments to a list of waypoints
        Route route = GetFollowSegmentsRoute(segments, endWaypoint);
        return route;
    }
}

