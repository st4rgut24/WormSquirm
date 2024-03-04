using System;
using UnityEngine;

/// <summary>
/// A line within a segment that defines the travelable space
/// </summary>
public class Guideline
{
	public Vector3 start;
	public Vector3 end;

    public Vector3 center;

    public Vector3 direction;

	public Guideline(Vector3 start, Vector3 end)
	{
		this.start = start;
		this.end = end;
        this.center = (start + end) / 2;

        this.direction = this.end - this.start;
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
        if (t <= 0.0f)
            return start;

        if (t >= lineLength)
            return end;

        // Calculate the closest point on the line segment
        Vector3 closestPoint = start + t * line;

        // Debug.Log("Closest point is " + closestPoint);
        return closestPoint;
    }
}

