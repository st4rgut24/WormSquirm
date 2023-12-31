﻿using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;


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
    public static int segmentDistance = 12; // length of the route in segments
    public static int waypointInterval = 3; // how many segments between waypoints

    /// <summary>
    /// Get a Route for a agent to follow
    /// </summary>
    /// <param name="strat">Strategy used for constructing a route</param>
    /// <param name="transform">Destination of the route</param>
    /// <returns></returns>
    public static Route Get(RouteStrat strat, Transform transform)
    {
        switch (strat)
        {
            case RouteStrat.FollowSegment:
                return FollowSegments(transform);
            case RouteStrat.StraightPath:
                return CreateStraightPath(transform);
            default:
                return FollowSegments(transform);
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
        route.AddWaypoint(BotManager.Instance.SimpStartBlock.transform.position);
        route.AddWaypoint(targetTransform.position);

        return route;
    }

    /// <summary>
    /// Create a route that reaches target by following existing segments
    /// </summary>
    /// <param name="targetTransform">the transform of the destination object</param>
    /// <returns>a route</returns>
    public static Route FollowSegments(Transform targetTransform)
    {
        GameObject tunnelGo = TunnelManager.Instance.GetGameObjectSegment(targetTransform);

        Segment goalSegment = TunnelManager.Instance.SegmentDict[tunnelGo.name];

        int dist = 1;

        Route route = new Route();
        Segment segment = goalSegment;
        while (dist <= segmentDistance)
        {
            if (dist % waypointInterval == 0)
            {
                route.AddWaypoint(segment.getCenter());
                List<GameObject> prevTunnels = segment.getPrevTunnels();

                if (prevTunnels.Count == 0)
                {
                    throw new Exception("Cannot follow segment, because missing previous tunnels");
                }

                GameObject firstPrevTunnel = prevTunnels[0];
                segment = TunnelManager.Instance.SegmentDict[firstPrevTunnel.name];
            }

            dist++;
        }

        return route;
    }
}

