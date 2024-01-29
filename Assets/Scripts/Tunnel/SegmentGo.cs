﻿using System;
using UnityEngine;


public class SegmentGo
{
	public Corridor corridor;
	public GameObject cap;


	public bool hasCap()
	{
		return cap != null;
	}

	public GameObject getTunnel()
	{
		return corridor.tunnel;
	}

	public float GetCorridorLength()
	{
		return corridor.GetLength();
	}

	public SegmentGo(GameObject cap, Corridor corridor)
	{
		this.corridor = corridor;
		this.cap = cap;
	}

	public void DestroyCap()
	{
        GameObject.Destroy(cap);
    }

	public void Destroy()
	{
		GameObject.Destroy(corridor.tunnel);
		DestroyCap();
	}
}

