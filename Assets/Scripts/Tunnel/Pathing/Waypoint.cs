using System;
using UnityEngine;

public class Waypoint
{
	public Vector3 position;
	public Segment segment;

	public Waypoint(Vector3 position, Segment segment = null)
	{
		this.position = position;
		this.segment = segment;
	}
}

