using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A series of waypoints from goal to destination
/// </summary>
public class Route
{
    int waypointIdx = -1; // the index of the waypoint along the route the user is currently traversing

    public List<Waypoint> waypoints;

    public class MiniRoute
	{
        public Segment startSegment;
        public Segment endSegment;

        public Vector3 start;
        public Vector3 end;

		public bool isFinalWaypoint;

		public MiniRoute(List<Waypoint> waypoints, int waypointIdx)
		{
			if (waypointIdx + 1 < waypoints.Count)
			{
				Waypoint startWaypoint = waypoints[waypointIdx];
				Waypoint endWaypoint = waypoints[waypointIdx + 1];

                this.start = startWaypoint.position;
                this.end = endWaypoint.position;

				this.startSegment = startWaypoint.segment;
				this.endSegment = endWaypoint.segment;

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
		waypoints = new List<Waypoint>();
	}

	public Segment GetInitSegment()
	{
		if (waypoints.Count > 0)
		{
			return waypoints[0].segment;
		}
		else
		{
			return null;
		}
	}

	public void AddWaypoint(Waypoint waypoint)
	{
		//waypoints.Insert(0, wp); // waypoints are added in reverse order from end to start
		waypoints.Add(waypoint);
	}

	public Vector3 getDestination()
	{
		if (waypoints.Count > 0)
		{
			Waypoint waypoint = waypoints[waypoints.Count - 1];

			return waypoint.position;
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

