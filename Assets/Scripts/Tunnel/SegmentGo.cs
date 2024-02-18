using System;
using System.Net.NetworkInformation;
using UnityEngine;


public class SegmentGo
{
	public Corridor corridor;

    public Cap EndCap;
    public Cap StartCap;

    public GameObject CapPrefab;

    public bool HasDeadEndCap()
    {
        return hasEndCap() && EndCap.meshType == MeshType.EndCap;
    }

    public bool hasEndCap()
	{
        return EndCap != null && EndCap.capObject != null;
	}

    public bool hasStartCap()
    {
        return StartCap != null && StartCap.capObject != null;
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

        GameObject EndCapObject = MeshObjectFactory.Get(MeshType.EndCap, CapPrefab, endRing, new OptionalMeshProps());
        EndCap = new Cap(EndCapObject, endRing, MeshType.EndCap);

        StartCap = new Cap(prevRing); // stsart of a segment is not capped
    }

    /// <summary>
    /// Cut a hole in a cap, the cap is destroyed, but must retain an ring to hide any deformities of intersected tunnel
    /// </summary>
    public void IntersectEndCap()
	{
		// replace the end cap with an intersected cap
		DestroyEndCap();
        GameObject endCapObject = MeshObjectFactory.Get(MeshType.PassThruCap, CapPrefab, EndCap.ring, new OptionalMeshProps());
        EndCap.SetCapObject(endCapObject, corridor.tunnel.transform, MeshType.PassThruCap);
    }

    public void IntersectStartCap()
    {
		DestroyStartCap();
        GameObject startCapObject = MeshObjectFactory.Get(MeshType.PassThruCap, CapPrefab, StartCap.ring, new OptionalMeshProps());
        StartCap.SetCapObject(startCapObject, corridor.tunnel.transform, MeshType.PassThruCap);
    }

    public GameObject GetEndCap()
	{
        return EndCap.capObject;
	}

	public void DestroyEndCap()
	{
		if (hasEndCap())
		{
            GameObject.Destroy(EndCap.capObject);
        }
    }

    public void DestroyStartCap()
    {
        if (hasStartCap())
        {
            GameObject.Destroy(StartCap.capObject);
        }
    }

    public void Destroy()
	{
		corridor.Destroy();
        DestroyStartCap();
        DestroyEndCap();
	}
}

