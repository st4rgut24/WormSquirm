using System;
using System.Collections.Generic;
using UnityEngine;

public class Segment
{
	Ring startRing;
	Ring endRing;

	public Segment(Ring startRing, Ring endRing)
	{
		this.startRing = startRing;
		this.endRing = endRing;
	}
}

