using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A series of waypoints from goal to destination
/// </summary>
public class Route
{
    int waypointIdx = -1; // the index of the waypoint along the route the user is currently traversing

    public List<Vector3> waypoints;

    public class MiniRoute
	{
        public Vector3 start;
        public Vector3 end;
        public bool isFinalWaypoint;

		public MiniRoute(List<Vector3> waypoints, int waypointIdx)
		{
			if (waypointIdx + 1 < waypoints.Count)
			{
                this.start = waypoints[waypointIdx];
                this.end = waypoints[waypointIdx + 1];

				isFinalWaypoint = waypointIdx + 1 == waypoints.Count - 1; // if true, this is the last stop to complete the overall Route
            }
			else
			{
				throw new Exception("Index exceeds length of waypoints array");
			}
        }
    }

	public Route()
	{
		waypoints = new List<Vector3>();
	}

	public void AddWaypoint(Vector3 wp)
	{
		waypoints.Add(wp);
	}

	public Vector3 getDestination()
	{
		if (waypoints.Count > 0)
		{
			return waypoints[waypoints.Count - 1];
		}
		else
		{
			throw new Exception("The route does not have any waypoints");
		}
	}

	/// <summary>
	/// Get the route within the larger route, consisting of a start and end location
	/// </summary>
	/// <returns>A mini route</returns>
	public MiniRoute GetNewMiniRoute()
	{
		waypointIdx++; // advance the index along the route
		return new MiniRoute(waypoints, waypointIdx);
	}

    public MiniRoute GetMiniRoute()
    {
        return new MiniRoute(waypoints, waypointIdx);
    }
}

