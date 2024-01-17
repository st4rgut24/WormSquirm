using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Segment
{
    public GameObject tunnel;

    public Ring endRing;
	public Vector3 endRingCenter;
	public Vector3 startRingCenter;
    public Vector3 forward;

    Vector3 center = DefaultUtils.DefaultVector3;
	List<GameObject> prevTunnel;
	List<GameObject> nextTunnel;

	public Segment(GameObject cur, GameObject prev, Ring ring, Ring prevRing)
	{
		this.startRingCenter = prevRing.GetCenter();
		this.endRingCenter = ring.GetCenter();
        this.endRing = ring;
        this.forward = (this.endRingCenter - this.startRingCenter).normalized;

		this.tunnel = cur;

		this.prevTunnel = new List<GameObject>();
		this.nextTunnel = new List<GameObject>();

		setPrevTunnel(prev);
	}

    public List<GameObject> getNextTunnels()
    {
        return this.nextTunnel;
    }

    public List<GameObject> getPrevTunnels()
	{
		return this.prevTunnel;
	}

    public Ring GetEndRing()
    {
        return endRing;
    }

    /// <summary>
    /// A tunnel that is not the first segment will have no end cap
    /// </summary>
    /// <returns>true if is a leading tunnel, false otherwise</returns>
    public bool hasEndCap()
    {
        return this.nextTunnel.Count == 0;
    }

    public void setPrevTunnel(GameObject prev)
    {
        this.prevTunnel.Add(prev);
    }

    public void setNextTunnels(List<GameObject> nexts)
    {
        this.nextTunnel.AddRange(nexts);
    }

    public void setNextTunnel(GameObject next)
	{
		this.nextTunnel.Add(next);
	}

	void setCenter()
	{
        MeshRenderer renderer = tunnel.GetComponent<MeshRenderer>();
        Vector3 center = renderer.bounds.center;
        this.center = center;
    }

	public Vector3 getCenter()
	{
		if (center == DefaultUtils.DefaultVector3)
		{
			setCenter();
		}

		return center;
	}

    /// <summary>
    /// Get the closest distance between point and centered line segment
    /// </summary>
    /// <param name="point">a point in world space</param>
    /// <returns>closest distance to line segment</returns>
	public float GetClosestDistanceToCenterLine(Vector3 point)
	{
        Vector3 line = endRingCenter - startRingCenter;
        float lineLength = line.magnitude;
        line.Normalize();

        Vector3 v = point - startRingCenter;
        float t = Vector3.Dot(v, line);

        // Check if the point is beyond the line segment
        if (t <= 0.0f)
            return Vector3.Distance(point, startRingCenter);

        if (t >= lineLength)
            return Vector3.Distance(point, endRingCenter);

        // Calculate the closest point on the line segment
        Vector3 closestPoint = startRingCenter + t * line;

        // Return the distance between the point and the closest point on the line segment
        return Vector3.Distance(point, closestPoint);

    }
}

