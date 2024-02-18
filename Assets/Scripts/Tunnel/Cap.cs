using System;
using UnityEngine;

public class Cap
{
	public GameObject capObject;
	public Ring ring;
	public MeshType meshType;

	public Cap(GameObject cap, Ring ring, MeshType meshType)
	{
		this.capObject = cap;
		this.ring = ring;
		this.meshType = meshType;
	}

    public Cap(Ring ring)
    {
        this.ring = ring;
    }

	/// <summary>
	/// Cap Gameobject exists
	/// </summary>
	/// <returns>true if has cap</returns>
	public bool HasCap()
	{
		return capObject != null;
	}

	public void SetCapObject(GameObject cap, Transform parentTransform, MeshType meshType)
	{
		this.capObject = cap;
		cap.transform.parent = parentTransform;
		this.meshType = meshType;
	}
}

