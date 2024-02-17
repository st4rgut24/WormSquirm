using System;
using UnityEngine;


public class SegmentGo
{
	public Corridor corridor;

    public GameObject CapPrefab;

    public bool hasCap()
	{
		return corridor.cap != null;
	}

	public GameObject getTunnel()
	{
		return corridor.tunnel;
	}

	public float GetCorridorLength()
	{
		return corridor.GetLength();
	}

    public SegmentGo(GameObject tunnelObject, GameObject CapPrefab, Ring endRing, Ring prevRing)
    {
		this.CapPrefab = CapPrefab;
		this.corridor = new Corridor(tunnelObject, CapPrefab, endRing, prevRing);
        //this.corridor = corridor;
        //this.cap = cap;
    }

	public void IntersectCap()
	{
		corridor.IntersectCap(CapPrefab);
	}

	public GameObject GetCap()
	{
		return corridor.cap;
	}

	public void DestroyCap()
	{
		corridor.DestroyCap();
    }

	public void Destroy()
	{
		corridor.Destroy();
	}
}

