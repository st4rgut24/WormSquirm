using System;
using UnityEngine;

/// <summary>
/// A line within a segment that defines the travelable space
/// </summary>
public class Guideline
{
	public Vector3 start;
	public Vector3 end;

	public Guideline(Vector3 start, Vector3 end)
	{
		this.start = start;
		this.end = end;
	}

    /// <summary>
    /// Get the closest point on this line to another point
    /// </summary>
    /// <param name="point">a point in world space</param>
    /// <returns>closest point on the line segment</returns>
    public Vector3 GetClosestPoint(Vector3 point)
    {
        Vector3 line = end - start;
        float lineLength = line.magnitude;
        line.Normalize();

        Vector3 v = point - start;
        float t = Vector3.Dot(v, line);

        // Check if the point is beyond the line segment
        //if (t <= 0.0f)
        //    return Vector3.Distance(point, startRingCenter);

        //if (t >= lineLength)
        //    return Vector3.Distance(point, endRingCenter);

        // Calculate the closest point on the line segment
        Vector3 closestPoint = start + t * line;

        return closestPoint;
    }
}

