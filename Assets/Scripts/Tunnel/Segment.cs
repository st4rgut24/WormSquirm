using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Segment
{
    public SegmentGo segmentGo;
    public GameObject tunnel;

    public Guideline centerLine;
    public Ring endRing;
    public Vector3 endRingCenter;
    public Vector3 startRingCenter;
    public float length;

    public Quaternion rotation {get; private set;}

    // directions are relative to tunnel's end ring
    public Vector3 forward;
    public Vector3 up;
    public Vector3 right;

    Vector3 center = DefaultUtils.DefaultVector3;

	List<GameObject> nextTunnel;

    List<Guideline> segmentLines;

    /// <summary>
    /// A wrapper for a Tunnel that contains positional information
    /// </summary>
    /// <param name="up">local up of the tunnel</param>
    /// <param name="segmentGo">segment gameobject wrapper</param>
    /// <param name="ring">a list of vertices defining end of segment</param>
    /// <param name="prevRing">a list of vertices defining beginning of segment</param>
	public Segment(Vector3 up, SegmentGo segmentGo, Ring ring, Ring prevRing)
	{
		this.startRingCenter = prevRing.GetCenter();
		this.endRingCenter = ring.GetCenter();
        this.endRing = ring;
        this.forward = (this.endRingCenter - this.startRingCenter).normalized;
        this.up = up;

        this.rotation = Quaternion.LookRotation(this.forward, this.up);
        this.right = Vector3.Cross(this.forward, this.up);

        this.length = Vector3.Distance(this.endRingCenter, this.startRingCenter);

        this.segmentGo = segmentGo;
        this.tunnel = this.segmentGo.getTunnel();

		this.nextTunnel = new List<GameObject>();

        this.segmentLines = new List<Guideline>();
        centerLine = new Guideline(this.startRingCenter, this.endRingCenter);
        Debug.DrawRay(centerLine.start, centerLine.end - centerLine.start, Color.green, 300);

        this.segmentLines.Add(centerLine);
    }

    public Vector3 GetUpDir()
    {
        return tunnel.transform.up;
    }

    /// <summary>
    /// Get the y position based off the center line
    /// </summary>
    /// <returns>elevation</returns>
    public float GetElevation()
    {
        Vector3 center = GetCenterLineCenter();
        return center.y;
    }

    /// <summary>
    /// Get rotation of tunnel
    /// </summary>
    public Quaternion GetRotation()
    {
        return segmentGo.GetRotation();
    }

    public void AddGuideline(Guideline line)
    {
        segmentLines.Add(line);
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

    public Cap GetEndCap()
    {
        return segmentGo.EndCap;
    }

    public Cap GetStartCap()
    {
        return segmentGo.StartCap;
    }

    /// <summary>
    /// A tunnel that is not the first segment will have no end cap
    /// </summary>
    /// <returns>true if is a leading tunnel, false otherwise</returns>
    public bool HasDeadEndCap()
    {
        return segmentGo.HasDeadEndCap();
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

    /// <summary>
    /// Get the point in the middle of the center guideline
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCenterLineCenter()
    {
        return centerLine.center;
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
    public float GetClosestDistance(Vector3 point, List<Guideline> guidelines)
    {
        float closestDist = Mathf.Infinity;
        Vector3 closestSegmentPoint = DefaultUtils.DefaultVector3;

        guidelines.ForEach((guideline) =>
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
    public bool IsOutOfBounds(Transform transform, Vector3 originalPos, Vector3 position)
    {
        if (HasDeadEndCap()) // check if player is close enough to the end of a tunnel
        {
            float DistToEndCap = GetClosestDistance(position, endRing.GetRingLines());
            // Debug.Log("In Tunnel " + tunnel.name + ". Projected Distance to end cap " + DistToEndCap + ". Should be less than " + SegmentManager.Instance.MinDistFromCap + " Current dist is " + curDistToEndCap);

            if (DistToEndCap <= SegmentManager.Instance.MinDistFromCap)
            {
                //Debug.Log("Distance to end cap is " + DistToEndCap + ". Player is out of bounds");
                return true;
            }
        }
        float dist = GetClosestDistance(position, segmentLines);
        //float curDist = GetClosestDistance(originalPos, segmentLines);
        // Debug.Log("In Tunnel " + tunnel.name + ". Projected Distance to center line is " + dist + ". Should be less than " + SegmentManager.Instance.MinDistFromCenterLine + " Current dist is " + curDist);
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

         //Debug.Log("Check if segment " + tunnel.name + " contains player. BehindStart is " + behindStart + " aheadOfEnd is " + aheadOfEnd + " isInbounds is " + isInbounds);

        return !behindStart && !aheadOfEnd && isInbounds;
    }
}

