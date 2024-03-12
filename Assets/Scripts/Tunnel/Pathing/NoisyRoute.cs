using System;
using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// Transforms an existing route by adding new waypionts to add noise to the route
/// </summary>
public class NoisyRoute : Route
{
    /// <summary>
    /// A Route with noise added to it
    /// </summary>
    /// <param name="waypoints">waypoints belonging to a standard route</param>
    /// <param name="transform">Transform of the agent traveling the route</param>
	public NoisyRoute(List<Waypoint> waypoints, Transform transform)
	{
        AddWaypoint(waypoints[0]);

        for (int r = 1; r < waypoints.Count; r++)
        {
            Waypoint wp = waypoints[r];
            Waypoint prevWP = waypoints[r - 1];

            Waypoint randomWP = GetNoisyWaypoint(wp, prevWP, transform);

            AddWaypoint(randomWP);
            AddWaypoint(waypoints[r]);
        }
    }

    /// <summary>
    /// Add a noisy intermediary waypoint between two 'normal' waypoints
    /// </summary>
    /// <param name="wp">current waypoint</param>
    /// <param name="prevWP">previous waypoint</param>
    /// <param name="transform">agent transform used for getting direction of noise vector</param>
    /// <returns>Waypoint off the beaten path</returns>
    private Waypoint GetNoisyWaypoint(Waypoint wp, Waypoint prevWP, Transform transform)
    {
        Vector3 walkDir = wp.position - prevWP.position;
        Vector3 intermediatePos = (wp.position + prevWP.position) / 2;

        Vector3 randomWalkDir = Vector3.Cross(transform.up, walkDir);
        float randomWalkDist = UnityEngine.Random.Range(-Consts.randomMoveRange, Consts.randomMoveRange);
        Vector3 randomPos = intermediatePos + randomWalkDir.normalized * randomWalkDist;

        Waypoint randomWP = new Waypoint(randomPos);

        return randomWP;
    }
}

