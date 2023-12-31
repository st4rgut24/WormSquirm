﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class Segment
{
	Vector3 center = DefaultUtils.DefaultVector3;

	GameObject tunnel;
	List<GameObject> prevTunnel;
	List<GameObject> nextTunnel;

	public Segment(GameObject cur, GameObject prev)
	{
		this.tunnel = cur;

		this.prevTunnel = new List<GameObject>();
		this.nextTunnel = new List<GameObject>();

		setPrevTunnel(prev);
	}

	public List<GameObject> getPrevTunnels()
	{
		return this.prevTunnel;
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
}

