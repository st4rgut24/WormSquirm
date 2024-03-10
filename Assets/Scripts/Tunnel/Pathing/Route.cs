using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A series of waypoints from goal to destination
/// </summary>
public class Route
{
    int waypointIdx = 0; // the index of the waypoint along the route the user is currently traversing

    public List<Waypoint> waypoints;

	public Route()
	{
		waypoints = new List<Waypoint>();
	}

	public int GetLength()
	{
		return waypoints.Count;
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

	/// <summary>
	/// Is the current waypoint the last waypoint in the route
	/// </summary>
	/// <returns>true if waypoint is final</returns>
	public bool IsFinalWaypoint(Waypoint waypoint)
	{
		Waypoint lastWP = GetLastWaypoint();
		return waypoint == lastWP;
	}

	public Vector3 GetCurrentPosition()
	{
		Waypoint curWP = GetCurWaypoint();
		return curWP.position;
	}

    public Waypoint GetNextWaypoint()
    {
        return waypoints[waypointIdx + 1];
    }

    public Waypoint GetCurWaypoint()
	{
		return waypoints[waypointIdx];
	}

    public void AdvanceWaypoint()
    {
		waypointIdx++;
    }

    public Waypoint GetLastWaypoint()
	{
		return waypoints[waypoints.Count - 1];
	}

	public void AddWaypoint(Waypoint waypoint)
	{
		//waypoints.Insert(0, wp); // waypoints are added in reverse order from end to start
		waypoints.Add(waypoint);
	}

	public void AddWaypoints(List<Waypoint> newWaypoints)
	{
		waypoints.AddRange(newWaypoints);
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
}

