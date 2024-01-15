using System;
using UnityEngine;


public class SegmentGo
{
	public Corridor corridor;
	public GameObject cap;


	public GameObject getTunnel()
	{
		return corridor.tunnel;
	}

	public SegmentGo(GameObject cap, Corridor corridor)
	{
		this.corridor = corridor;
		this.cap = cap;
	}
}

