using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Waypoint
{
	public Vector3 position;
	public Segment segment;

    /// <summary>
    /// A marker along a route
    /// </summary>
    /// <param name="position">position of waypoint</param>
    /// <param name="segment">the destination segment of the waypoint</param>
	public Waypoint(Vector3 position, Segment segment = null)
	{
		this.position = position;
		this.segment = segment;
	}

    public override bool Equals(object obj)
    {
        if (obj is Waypoint other)
        {
            return position == other.position;
        }
        return false;
    }

    public override int GetHashCode()
    {
        // Combine hash codes of properties
        return position.GetHashCode();
    }
}

