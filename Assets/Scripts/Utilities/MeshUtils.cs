using System;
using System.Linq;
using UnityEngine;

public class MeshUtils
{
    public static int[] FlipNormals(Mesh mesh)
    {
        return mesh.triangles.Reverse().ToArray();
    }
}

