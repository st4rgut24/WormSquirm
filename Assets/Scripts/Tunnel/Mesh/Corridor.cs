using System;
using UnityEngine;

public class Corridor
{
    public GameObject tunnel;

	public float length;

	public Corridor(GameObject corridor, GameObject CapPrefab, Ring ring, Ring prevRing)
	{
		this.tunnel = corridor;

        this.length = Vector3.Distance(ring.GetCenter(), prevRing.GetCenter());
    }

    public void Destroy()
    {
        GameObject.Destroy(tunnel);
    }

    public float GetLength()
	{
		return this.length;
	}
}

