using UnityEngine;

public class CapCreator : MonoBehaviour
{
    public int segments = 4; // Number of segments in each ring
    public float radiusOuter = 5f; // Outer ring radius
    public float radiusInner = 3f; // Inner ring radius

    public Vector3 direction = new Vector3(0, 1);
    public Vector3 position = new Vector3(0, 0);

    void Start()
    {
        //Mesh mesh = GenerateMesh();
        Mesh mesh = GeneratePassCapMesh();

        GetComponent<MeshFilter>().mesh = mesh;
    }

    Mesh GeneratePassCapMesh()
    {
        Ring outerRing = RingFactory.Create(radiusOuter, segments, direction, position, 0);
        Ring innerRing = RingFactory.Create(radiusInner, segments, direction, position, 0);

        PassCap passCap = new PassCap(outerRing, innerRing);

        return passCap.GetMesh();
    }

    Mesh GenerateMesh()
    {
        Mesh mesh = new Mesh();

        // Vertices
        Vector3[] vertices = new Vector3[2 * segments];
        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * 2 * Mathf.PI;
            float xOuter = Mathf.Cos(angle) * radiusOuter;
            float zOuter = Mathf.Sin(angle) * radiusOuter;

            float xInner = Mathf.Cos(angle) * radiusInner;
            float zInner = Mathf.Sin(angle) * radiusInner;

            vertices[i] = new Vector3(xOuter, 0f, zOuter); // Outer ring vertices
            vertices[i + segments] = new Vector3(xInner, 0f, zInner); // Inner ring vertices
        }

        // Triangles
        int[] triangles = new int[6 * segments];
        for (int i = 0, ti = 0; i < segments - 1; i++, ti += 6)
        {
            triangles[ti] = i;
            triangles[ti + 1] = i + segments;
            triangles[ti + 2] = i + 1;

            triangles[ti + 3] = i + 1;
            triangles[ti + 4] = i + segments;
            triangles[ti + 5] = i + segments + 1;
        }

        // Connect the last segment
        triangles[triangles.Length - 6] = segments - 1;
        triangles[triangles.Length - 5] = 2 * segments - 1;
        triangles[triangles.Length - 4] = 0;

        triangles[triangles.Length - 3] = 0;
        triangles[triangles.Length - 2] = 2 * segments - 1;
        triangles[triangles.Length - 1] = segments;

        // Assign vertices and triangles to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Recalculate normals to ensure lighting works correctly
        mesh.RecalculateNormals();

        return mesh;
    }
}
