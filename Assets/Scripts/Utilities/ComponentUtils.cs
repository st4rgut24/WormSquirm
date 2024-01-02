using System;
using System.Collections.Generic;
using UnityEngine;

public class ComponentUtils: MonoBehaviour
{
    public static Mesh GetMesh(GameObject obj)
    {
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        return meshFilter.mesh;

    }

    public static List<Mesh> GetMeshes(List<GameObject>objects)
    {
        List<Mesh> meshList = new List<Mesh>();

        objects.ForEach((obj) =>
        {
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            meshList.Add(meshFilter.mesh);
        });

        return meshList;
    }

    public static void removeMeshCollider(GameObject obj)
    {
        MeshCollider meshCollider = obj.GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            Destroy(meshCollider);
        }
    }

    public static void removeMeshColliders(List<GameObject> objects)
    {
        objects.ForEach((obj) =>
        {
            MeshCollider meshCollider = obj.GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                Destroy(meshCollider);
            }
        });
    }

    public static void addMeshColliders(List<GameObject> objects)
    {
        objects.ForEach((obj) =>
        {
            MeshCollider meshCollider = obj.GetComponent<MeshCollider>();

            if (meshCollider == null)
            {
                obj.AddComponent<MeshCollider>();
            }
        });
    }
}

