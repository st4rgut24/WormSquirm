using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Net.NetworkInformation;

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
	Tunnel,
    PassThruCap
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

        Debug.Log("Testing");

        switch (type)
		{
			case MeshType.EndCap:
				mesh = CreateEndCapMesh(ring);
				break;
            case MeshType.PassThruCap:
                mesh = CreatePassCap(ring);
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

        //Vector3[] vertices = prevRing.vertices.Concat(ring.vertices).ToArray();

        int verticesPerRing = ring.vertices.Length;
        int verticesPerTriangle = 6;

        //tunnelMesh.vertices = vertices;

        List<Ring> rings = RingFactory.CreateRings(prevRing, ring);
        int slices = rings.Count - 1;

        if (verticesPerRing != props.TunnelSides)
        {
            throw new Exception("vertex count per ring not equal to segment count");
        }
        if (slices < 1)
        {
            throw new Exception("cant create a tunnel with less than one slice");
        }
        Vector3[] vertices = new Vector3[rings.Count * verticesPerRing];
        int[] triangles = new int[verticesPerTriangle * props.TunnelSides * slices];

        // build vertices
        for (int r = 0; r < rings.Count; r++)
        {
            Ring offsetRing = rings[r];

            int offset = r * verticesPerRing;

            Array.Copy(offsetRing.vertices, 0, vertices, offset, verticesPerRing);
        }

        // build triangles
        int ti = 0;

        for (int r = 0; r < rings.Count - 1; r++)
        {
            Ring offsetRing = rings[r];

            int offset = r * verticesPerRing;

            int oppFaceOffset = offset + verticesPerRing; // vertex index offset for the first vertex on the opposite ring

            // add a tunnel segment to the mesh between each pair of rings
            CreateTunnelSlice(offset, oppFaceOffset, ref ti, triangles, verticesPerRing, verticesPerTriangle);
        }

        tunnelMesh.vertices = vertices;
        tunnelMesh.triangles = triangles;

        return tunnelMesh;
    }

    /// <summary>
    /// Create a segment of the tunnel
    /// </summary>
    /// <param name="offset">mesh vertex offset for this slice</param>
    /// <param name="oppFaceOffset">offset for the starting vertex of the opposite ring of the slice</param>
    /// <param name="ti">triangle vertex index</param>
    /// <param name="triangles">triangles array for the mesh</param>
    /// <param name="verticesPerRing">vertices per ring</param>
    /// <param name="verticesPerTriangle">vertices per triangle</param>
    private static void CreateTunnelSlice(int offset, int oppFaceOffset, ref int ti, int[] triangles, int verticesPerRing, int verticesPerTriangle)
    {
        for (int i = offset; i < oppFaceOffset; i++, ti += verticesPerTriangle)
        {
            // last tunnel segment (rectangle) connects the first triangle to the last triangle
            if (i == oppFaceOffset - 1)
            {
                triangles[ti] = i;
                triangles[ti + 1] = offset;
                triangles[ti + 2] = oppFaceOffset;

                triangles[ti + 3] = oppFaceOffset;
                triangles[ti + 4] = i + verticesPerRing;
                triangles[ti + 5] = i;
            }
            else
            {
                triangles[ti] = i;
                triangles[ti + 1] = i + 1;
                triangles[ti + 2] = i + verticesPerRing;

                triangles[ti + 3] = i + 1;
                triangles[ti + 4] = i + verticesPerRing + 1;
                triangles[ti + 5] = i + verticesPerRing;
            }
        }
    }

    /// <summary>
    /// Add a segment to a tunnel mesh, defined as the faces between two rings
    /// </summary>
    private static void AddTunnelMeshSegment(Ring startRing, Ring endRing, Vector3[] vertices, int[] triangles)
    {
        vertices = vertices.Concat(endRing.vertices).ToArray();

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

    /// <summary>
    /// Generate a cap that has a hole in the middle
    /// </summary>
    /// <param name="ring">the vertices of the original (whole) cap</param>
    /// <returns>A mesh for the cap/returns>
    private static Mesh CreatePassCap(Ring ring)
    {
        float innerRadius = ring.radius - 2f;

        Ring innerRing = RingFactory.Create(innerRadius, ring.vertices.Length, ring.normal, ring.center, 0);
        PassCap passCap = new PassCap(ring, innerRing);
        return passCap.GetMesh();
    }
}

