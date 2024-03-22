using System;
using UnityEngine;

/// <summary>
/// Determines the position of a key
/// </summary>
public class Key
{
	public Vector3 position { get; private set; }

    /// <summary>
    /// Spawn a key at an angle to perpendicular to the provided segment
    /// </summary>
    /// <param name="minAngle">lower bound of </param>
    /// <param name="maxAngle"></param>
    public Key(Segment segment, int distance, float minAngle, float maxAngle)
	{
		KeyRange range = new KeyRange(minAngle, maxAngle);
		float angle = range.GetAngle();
		Debug.Log("Key angle " + angle);
		position = SegmentUtils.GetPerpendicularPoint(segment, distance, angle);
    }

    /// <summary>
    /// Spawn a key in the specified direction
    /// </summary>
    public Key(Segment segment, int distance, Vector3 direction)
    {
        position = SegmentUtils.GetCollinearPoint(segment,distance, direction);
    }
}

