using System;
using UnityEngine;

public class Heading
{
	public Vector3 position;
	public Vector3 forward;

	public Heading(Vector3 pos, Vector3 forward)
	{
		this.position = pos;
		this.forward = forward;
	}
}

