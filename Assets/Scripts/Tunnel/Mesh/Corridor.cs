using System;
using UnityEngine;

public class Corridor
{
	public Ring ring;
    public Ring prevRing;
    public GameObject tunnel;

	public Corridor(GameObject corridor, Ring ring, Ring prevRing)
	{
		this.tunnel = corridor;

		this.ring = ring;
		this.prevRing = prevRing;
	}
}

