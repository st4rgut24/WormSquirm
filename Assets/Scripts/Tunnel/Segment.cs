using System;
using System.Collections.Generic;
using UnityEngine;

public class Segment
{
    public SegmentGo segmentGo;
    public GameObject tunnel;

    public Guideline centerLine;
    public Ring endRing;
	public Vector3 endRingCenter;
	public Vector3 startRingCenter;
    public Vector3 forward;

    Vector3 center = DefaultUtils.DefaultVector3;
    // TODO: we only need nextTunnels list, prevTunnels list complicates things and is unnecessary
	//List<GameObject> prevTunnel;
	List<GameObject> nextTunnel;

    List<Guideline> guidelineList;

	public Segment(SegmentGo segmentGo, Ring ring, Ring prevRing)
	{
		this.startRingCenter = prevRing.GetCenter();
		this.endRingCenter = ring.GetCenter();
        this.endRing = ring;
        this.forward = (this.endRingCenter - this.startRingCenter).normalized;

        this.segmentGo = segmentGo;
        this.tunnel = this.segmentGo.getTunnel();

		this.nextTunnel = new List<GameObject>();

        this.guidelineList = new List<Guideline>();
        centerLine = new Guideline(this.startRingCenter, this.endRingCenter);
        Debug.DrawRay(centerLine.start, centerLine.end - centerLine.start, Color.green, 300);

        this.guidelineList.Add(centerLine);
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
        return segmentGo.hasCap();
    }

    //public void setPrevTunnel(GameObject prev)
    //{
    //    if (prev == null)
    //    {
    //        throw new Exception("A null object is added to list of previous tunnels");
    //    }
    //    this.prevTunnel.Add(prev);
    //}

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
    /// Get a point on edge of tunnel that will be the start ring of a bisecting segment
    /// </summary>
    /// <param name="heading">position and direction of bisecting transform</param>
    /// <returns>point of intersection</returns>
    public Vector3 GetIntersectionPoint(Heading heading)
    {
        // TODO: add mesh collider here, find where the mesh is intersected using Physics.Raycast
        Vector3 centerPoint = GetClosestPointToCenterline(heading.position);
        Vector3 edgePoint = centerPoint + heading.forward.normalized * TunnelManager.tunnelRadius;

        return edgePoint;
    }

    /// <summary>
    /// Get the closest distance between point and centered line segment
    /// </summary>
    /// <param name="point">a point in world space</param>
    /// <returns>closest distance to line segment</returns>
	public float GetDistanceToCenterLine(Vector3 point)
    {
        Vector3 segmentPoint = centerLine.GetClosestPoint(point);
        return Vector3.Distance(point, segmentPoint);
    }

    /// <summary>
    /// Get the closest distance to any guideline in the segment
    /// </summary>
    /// <param name="point">point</param>
    /// <returns></returns>
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

            //Debug.Log("Distance to end cap is " + distToEndCap + ". Min dist to end cap is " + SegmentManager.Instance.MinDistFromCap);
            // TODO: There is a situation where an intersecting tunnel is within the min distance from cap, which immobilizes the player
            // to remedy this, and situations like this, certain intersections should not be possible (perhaps by creating a unbreakable endcap?)
            if (distToEndCap <= SegmentManager.Instance.MinDistFromCap)
            {
                return true;
            }
        }
        float dist = GetClosestDistance(position);

        Debug.Log("In Tunnel " + tunnel.name + ". Distance to center line is " + dist + ". Should be less than " + SegmentManager.Instance.MinDistFromCenterLine);
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
        // TODO: inbounds is not a good indicator because guidelines may not be centered in the tunnel
        bool isInbounds = GetDistanceToCenterLine(transform.position) < TunnelManager.tunnelRadius;

        Debug.Log("Check if segment " + tunnel.name + " contains player. BehindStart is " + behindStart + " aheadOfEnd is " + aheadOfEnd + " isInbounds is " + isInbounds);

        return !behindStart && !aheadOfEnd && isInbounds;

    }
}

