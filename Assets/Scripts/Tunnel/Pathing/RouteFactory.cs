using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;


public enum RouteStrat
{
    FollowSegment,
    StraightPath,
    Gravity,
    None
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
    public static Route Get(RouteStrat strat, Automaton agent, Transform targetTransform, bool addNoise = false)
    {
        Route route;

        switch (strat)
        {
            case RouteStrat.FollowSegment:
                route = FollowSegments(agent.isFirstRoute(), agent.curSegment, targetTransform, agent.transform);
                break;
            case RouteStrat.StraightPath:
                route = CreateStraightPath(targetTransform);
                break;
            case RouteStrat.Gravity:
                route = CreateDownhillRoute(agent.isFirstRoute(), agent.curSegment);
                break;
            default:
                route = FollowSegments(agent.isFirstRoute(), agent.curSegment, targetTransform, agent.transform);
                break;
        }

        Route agentRoute = shouldAddNoise(route, addNoise) ? new NoisyRoute(route.waypoints, agent.transform) : route;

        return agentRoute;
    }

    /// <summary>
    /// Criteria for adding noise
    /// If there are only 2 waypoints (start and end), don't add noise to prevent weird routing behavior near objective colliders
    /// </summary>
    /// <param name="route">the candidate noisy route</param>
    /// <param name="addNoise">if the type of route is noisy</param>
    /// <returns></returns>
    public static bool shouldAddNoise(Route route, bool addNoise)
    {
        return addNoise && route.waypoints.Count > 2;
    }

    /// <summary>
    /// Creates a route consisting of a straight path between two points
    /// </summary>
    /// <param name="targetTransform">destination</param>
    /// <returns>a simple route</returns>
    public static Route CreateStraightPath(Transform targetTransform)
    {
        Route route = new Route();
        Transform[] simpWPs = BotManager.Instance.SimpWPs;

        for (int i = 0; i < simpWPs.Length; i++)
        {
            route.AddWaypoint(new Waypoint(simpWPs[i].position));
        }

        route.AddWaypoint(new Waypoint(targetTransform.position));

        return route;
    }

    public static List<Segment> CreateDownhillPath(Segment startSegment)
    {
        Segment segment = startSegment;
        List<Segment> downhillSegmentPath = new List<Segment>();
        HashSet<GameObject> SeenTunnels = new HashSet<GameObject>();

        // while there is a segment that is active keep adding to the downhill path
        while (IsNewActiveTunnel(segment?.tunnel, SeenTunnels))
        {
            SeenTunnels.Add(segment.tunnel);
            downhillSegmentPath.Add(segment);

            segment = SegmentUtils.GetNextTunnelWithSteepestDescent(segment);
        }

        return downhillSegmentPath;
    }

    private static bool IsNewActiveTunnel(GameObject tunnel, HashSet<GameObject> SeenTunnels)
    {
        return tunnel != null && tunnel.activeSelf && !SeenTunnels.Contains(tunnel);
    }

    /// <summary>
    /// Get the route that traverses segments
    /// </summary>
    /// <param name="spawnRoute">whether this is agent's first route, meaning agent will spawn at start of this route</param>
    /// <param name="segments">The segments that are part of the route</param>
    /// <param name="endWaypoint">The final waypoint</param>
    /// <returns>the route</returns>
    public static Route GetFollowSegmentsRoute(bool spawnRoute, List<Segment> segments, Waypoint endWaypoint)
    {
        Route route = new Route();

        for (int i = 1; i < segments.Count; i++)
        {
            Segment startSegment = segments[i - 1];
            Segment destSegment = segments[i];

            List<Waypoint> path = SegmentManager.Instance.GetConnectedSegmentPath(startSegment, destSegment);
            route.AddWaypoints(path);
        }

        // if this is the first route, choose a neutral spawning location (center of segment) instead of spawning directly at end waypoint
        if (spawnRoute && segments.Count == 1)
        {
            Vector3 segmentCenter = segments[0].GetCenterLineCenter();
            Waypoint startWP = new Waypoint(segmentCenter);
            route.AddWaypoint(startWP);
        }

        route.AddWaypoint(endWaypoint);

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
        while (dist <= defaultSegmentDist)
        {
            segments.Add(segment);

            List<GameObject> neighborTunnels = segment.getNextTunnels();

            if (neighborTunnels.Count == 0)
            {
                throw new Exception("Cannot follow segment, because missing previous tunnels");
            }

            GameObject prevTunnel = null;

            for (int i = 0; i < neighborTunnels.Count; i++)
            {
                GameObject neighborTunnel = neighborTunnels[i];

                // make sure the path does not go in loops, and the tunnels in the path are currently active
                if (IsNewActiveTunnel(neighborTunnel, seenTunnels))
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

    public static Route CreateDownhillRoute(bool isSpawnRoute, Segment startSegment)
    {
        List<Segment> DownhillSegments = CreateDownhillPath(startSegment);
        Segment destSegment = DownhillSegments[DownhillSegments.Count - 1];

        Waypoint endWaypoint = new Waypoint(destSegment.GetCenterLineCenter(), destSegment);
        Route route = GetFollowSegmentsRoute(isSpawnRoute, DownhillSegments, endWaypoint);

        return route;
    }

    /// <summary>
    /// Create a route that reaches target by following existing segments
    /// </summary>
    /// <param name="curSegment">current segment of the agent being assigned a route</param>
    /// <param name="targetTransform">the transform of the target agent</param>
    /// <param name="transform">transform of the agent being assigned route</param>
    /// <returns>a route ending up in the segment that target gameobject is in</returns>
    public static Route FollowSegments(bool spawnRoute, Segment curSegment, Transform targetTransform, Transform transform)
    {
        Segment goalSegment = AgentManager.Instance.GetSegment(targetTransform);
        bool isInSegment = curSegment != null;

        List<Segment> segments = isInSegment ?
            SearchUtils.dfsConnectSegments(goalSegment, curSegment) :
            StartFollowSegments(goalSegment);

        // convert a list of segments to a list of waypoints
        Waypoint endWP = new Waypoint(targetTransform.position);
        Route route = GetFollowSegmentsRoute(spawnRoute, segments, endWP);

        return route;
    }
}

