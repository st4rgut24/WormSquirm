using System;
using UnityEngine;

public class HitInfo
{
	public GameObject hitGo;
	public Vector3 hitCoord;

	public HitInfo(GameObject hitGo, Vector3 hitCoord)
	{
		this.hitGo = hitGo;
		this.hitCoord = hitCoord;
	}
}

