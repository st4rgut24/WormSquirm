using System;
using UnityEngine;

public class Corridor
{
	public Ring ring;
    public Ring prevRing;
    public GameObject tunnel;

	public float length;

	public Corridor(GameObject corridor, Ring ring, Ring prevRing)
	{
		this.tunnel = corridor;

		this.ring = ring;
		this.prevRing = prevRing;

		this.length = Vector3.Distance(this.ring.GetCenter(), this.prevRing.GetCenter());
	}

	public float GetLength()
	{
		return this.length;
	}
}

