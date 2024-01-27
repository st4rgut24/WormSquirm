using System;
using System.Collections.Generic;
using UnityEngine;

public class Segment
{
    public GameObject tunnel;

    public Guideline centerLine;
    public Ring endRing;
	public Vector3 endRingCenter;
	public Vector3 startRingCenter;
    public Vector3 forward;

    Vector3 center = DefaultUtils.DefaultVector3;
	List<GameObject> prevTunnel;
	List<GameObject> nextTunnel;

    List<Guideline> guidelineList;

	public Segment(GameObject cur, GameObject prev, Ring ring, Ring prevRing)
	{
		this.startRingCenter = prevRing.GetCenter();
		this.endRingCenter = ring.GetCenter();
        this.endRing = ring;
        this.forward = (this.endRingCenter - this.startRingCenter).normalized;

		this.tunnel = cur;

		this.prevTunnel = new List<GameObject>();
		this.nextTunnel = new List<GameObject>();

        this.guidelineList = new List<Guideline>();
        centerLine = new Guideline(this.startRingCenter, this.endRingCenter);
        Debug.DrawRay(centerLine.start, centerLine.end - centerLine.start, Color.green, 100);

        this.guidelineList.Add(centerLine);

        if (prev != null)
        {
            setPrevTunnel(prev);
        }
    }

    public void AddGuideline(Guideline line)
    {
        guidelineList.Add(line);
    }

    public Vector3 GetClosestPointToCenterline(Vector3 otherPoint)
    {
        return centerLine.GetClosestPoint(otherPoint);
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
        if (prev == null)
        {
            throw new Exception("A null object is added to list of previous tunnels");
        }
        this.prevTunnel.Add(prev);
    }

    public void setNextTunnels(List<GameObject> nexts)
    {
        if (nexts.Contains(null))
        {
            throw new Exception("A null object is added to list of next tunnels");
        }
        this.nextTunnel.AddRange(nexts);
    }

    public void setNextTunnel(GameObject next)
	{
        if (next == null)
        {
            throw new Exception("A null object is added to list of next tunnels");
        }
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
    /// Get the closest distance between point and centered line segment(s)
    /// </summary>
    /// <param name="point">a point in world space</param>
    /// <returns>closest distance to line segment</returns>
	public float GetClosestDistance(Vector3 point)
	{
        float closestDist = Mathf.Infinity;
        Vector3 closestSegmentPoint = DefaultUtils.DefaultVector3;

        guidelineList.ForEach((guideline) =>
        {
            Vector3 segmentPoint = guideline.GetClosestPoint(point);
            float dist = Vector3.Distance(point, segmentPoint);

            if (dist < closestDist)
            {
                closestSegmentPoint = segmentPoint;
                closestDist = dist;
            }
        });

        // Return the distance between the point and the closest point on the line segment
        return closestDist;
    }

    /// <summary>
    /// Check if the transform is outside segment bounds
    /// </summary>
    /// <param name="transform">transform</param>
    /// <param name="position">position to check for bounds</param>
    /// <returns>true if out of bounds</returns>
    public bool isOutOfBounds(Transform transform, Vector3 position)
    {
        if (hasEndCap()) // check if player is close enough to the end of a tunnel
        {
            float distToEndCap = Vector3.Distance(position, endRingCenter);

            Debug.Log("Distance to end cap is " + distToEndCap + ". Min dist to end cap is " + SegmentManager.Instance.MinDistFromCap);
            if (distToEndCap <= SegmentManager.Instance.MinDistFromCap)
            {
                return true;
            }
        }
        float dist = GetClosestDistance(position);

        //Debug.Log("In Tunnel " + tunnel.name + ". Distance to center line is " + dist + ". Should be less than " + SegmentManager.Instance.MinDistFromCenterLine);
        return dist >= SegmentManager.Instance.MinDistFromCenterLine; // divide by 2 because the moveable area is smaller on bottom plane of the tunnel
    }

    /// <summary>
    /// Detect whether a transform is within a segment
    /// </summary>
    /// <param name="transform">the moving thing</param>
    /// <returns>true if contains it</returns>
    public bool ContainsTransform(Transform transform)
    {
        bool behindStart = Vector3.Dot(forward, transform.position - startRingCenter) < 0;
        bool aheadOfEnd = Vector3.Dot(forward, transform.position - endRingCenter) > 0;
        return !behindStart && !aheadOfEnd;
    }
}

