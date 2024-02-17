using System;
using System.Net.NetworkInformation;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;
using static UnityEditor.Rendering.CameraUI;

public class Corridor
{
	public GameObject cap;

	public Ring CapRing;
    public Ring prevRing;
    public GameObject tunnel;

	public float length;

	public Corridor(GameObject corridor, GameObject CapPrefab, Ring ring, Ring prevRing)
	{
		this.tunnel = corridor;

		this.CapRing = ring;
		this.prevRing = prevRing;
        this.length = Vector3.Distance(this.CapRing.GetCenter(), this.prevRing.GetCenter());

        this.cap = MeshObjectFactory.Get(MeshType.EndCap, CapPrefab, ring, new OptionalMeshProps());
    }

    public void Destroy()
    {
		DestroyCap();
        GameObject.Destroy(tunnel);
    }

	/// <summary>
	/// Cut a hole in a cap, the cap is destroyed, but must retain an ring to hide any deformities of intersected tunnel
	/// </summary>
	/// <param name="CapPrefab">the ring representing a cap with a hole</param>
    public void IntersectCap(GameObject CapPrefab)
    {
		// replace the cap with a cap with a hole in it
        MeshObjectFactory.Get(MeshType.PassThruCap, CapPrefab, CapRing, new OptionalMeshProps());
        DestroyCap();
    }

    public void DestroyCap()
	{
		GameObject.Destroy(cap);
	}

    public float GetLength()
	{
		return this.length;
	}
}

