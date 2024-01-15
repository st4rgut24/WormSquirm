using System;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UIElements;

public struct OptionalMeshProps
{
    public OptionalMeshProps(Transform transform, Ring prevRing, TunnelProps props)
    {
        this.transform = transform;
        this.prevRing = prevRing;
        this.props = props;
    }

    public Transform transform;
    public Ring prevRing;
    public TunnelProps props;
}

public enum MeshType
{
	EndCap,
	Tunnel
}

/// <summary>
/// Create a mesh
/// </summary>
public class MeshObjectFactory
{
	/// <summary>
	/// Get a mesh object by type
	/// </summary>
	/// <param name="type">the type of mesh</param>
	/// <param name="prefab">the prefab to instantiate</param>
	/// <param name="transform">transform of player used for looking up meshes</param>
	/// <param name="ring">Ring used to create mesh</param>
	/// <returns></returns>
	public static GameObject Get(MeshType type, GameObject prefab, Ring ring, OptionalMeshProps meshProps)
	{
		GameObject MeshObject = GameObject.Instantiate(prefab);

		Mesh mesh;

		switch (type)
		{
			case MeshType.EndCap:
				mesh = CreateEndCapMesh(ring);
				break;
			case MeshType.Tunnel:
				mesh = CreateTunnelMesh(meshProps.transform, ring, meshProps.prevRing, meshProps.props);
				break;
			default:
				throw new Exception("Not a valid mesh type: " + type);
		}

        mesh.RecalculateNormals();
        //mesh.triangles = MeshUtils.FlipNormals(mesh);
        for (int i = 0; i < mesh.normals.Length; i++)
        {
            mesh.normals[i] = -mesh.normals[i];
        }

        //mesh.RecalculateNormals();
        //MeshUtils.InvertFaces(mesh);

        //MeshFilter meshFilter = MeshObject.GetComponent<MeshFilter>();
        //meshFilter.mesh = mesh;
        MeshObject.GetComponent<MeshFilter>().mesh = mesh;

        return MeshObject;
	}

    /// <summary>
    /// Create a Tunnel mesh
    /// </summary>
    /// <param name="transform">The key used to look up previous rings</param>
    /// <param name="ring">The current tunnel ring</param>
    /// <param name="props">Props about the tunnel</param>
    /// <returns>Mesh for the tunnel</returns>
	private static Mesh CreateTunnelMesh(Transform transform, Ring ring, Ring prevRing, TunnelProps props)
	{
        Mesh tunnelMesh = new Mesh();

        Vector3[] vertices = prevRing.vertices.Concat(ring.vertices).ToArray();

        int tunnelSegments = props.TunnelSegments;

        tunnelMesh.vertices = vertices;

        int[] triangles = new int[6 * tunnelSegments];

        for (int i = 0, ti = 0; i < tunnelSegments; i++, ti += 6)
        {
            // last tunnel segment connects the first triangle to the last triangle
            if (i == tunnelSegments - 1)
            {
                triangles[ti] = i;
                triangles[ti + 1] = 0;
                triangles[ti + 2] = tunnelSegments;

                triangles[ti + 3] = tunnelSegments;
                triangles[ti + 4] = i + tunnelSegments;
                triangles[ti + 5] = i;
            }
            else
            {
                triangles[ti] = i;
                triangles[ti + 1] = i + 1;
                triangles[ti + 2] = i + tunnelSegments;

                triangles[ti + 3] = i + 1;
                triangles[ti + 4] = i + tunnelSegments + 1;
                triangles[ti + 5] = i + tunnelSegments;
            }
        }
        tunnelMesh.triangles = triangles;

        RingManager.Instance.UpdateEntry(transform, ring);

        return tunnelMesh;
    }

    /// <summary>
    /// Generate a cap for the end of the tunnel
    /// </summary>
    /// <param name="ring">the vertices of the cap</param>
    /// <returns>A mesh for the cap/returns>
    private static Mesh CreateEndCapMesh(Ring ring)
    {
		EndCap endCap = new EndCap(ring);
		return endCap.GetMesh();
    }
}

