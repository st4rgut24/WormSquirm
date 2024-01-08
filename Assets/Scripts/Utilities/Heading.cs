using System;
using UnityEngine;

public class Heading
{
	public Vector3 position;
	public Vector3 forward;

	public Heading(Transform transform)
	{
		this.position = transform.position;
		this.forward = transform.forward;
	}
}

