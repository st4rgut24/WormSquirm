using System;
using System.Collections.Generic;
using UnityEngine;

public class InvertedTunnelDelete : TunnelDelete
{
    public InvertedTunnelDelete(GameObject Tunnel, List<Ray> rays) : base(Tunnel, rays)
    {

    }

    public override bool DeleteTunnel()
    {
        int[] flippedFaces = MeshUtils.FlipNormals(mesh);

        mesh.triangles = flippedFaces;

        bool isDeleted = base.DeleteTunnel();

        int[] originalFaces = MeshUtils.FlipNormals(mesh);

        mesh.triangles = originalFaces;

        return isDeleted;
    }
}

