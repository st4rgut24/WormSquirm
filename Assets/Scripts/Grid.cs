using UnityEngine;
using System.Collections.Generic;
using static UnityEditor.PlayerSettings;

public class Grid
{
    Vector3Int dimension;

    public int unitsPerCell = 4; // world units per array indice
                                 // minimum should be four times max dimension of a gameobject

    private List<GameObject>[,,] grid;

    public Grid() {
        this.dimension = getDimension();
        grid = new List<GameObject>[this.dimension.x, this.dimension.y, this.dimension.z];
    }

    Vector3Int ConvertWorldPosToGridPos(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / unitsPerCell);
        int y = Mathf.FloorToInt(worldPos.y / unitsPerCell);
        int z = Mathf.FloorToInt(worldPos.z / unitsPerCell);

        return new Vector3Int(x, y, z);
    }

    Vector3Int getDimension()
    {
        // Calculate the dimensions of the grid based on resolution
        int width = Mathf.CeilToInt((float)GameManager.Instance.xAxisLength / unitsPerCell);
        int length = Mathf.CeilToInt((float)GameManager.Instance.zAxisLength / unitsPerCell);
        int height = Mathf.CeilToInt((float)GameManager.Instance.yAxisLength / unitsPerCell);

        return new Vector3Int(width, length, height); 
    }

    public void AddGameObject(Vector3 pos, GameObject go)
    {
        if (!GameManager.Instance.isValidPos(pos))
        {
            throw new System.Exception("GameObject is out of bounds");
        }

        Vector3Int cellPos = ConvertWorldPosToGridPos(pos);

        List<GameObject> gameObjects = grid[cellPos.x, cellPos.y, cellPos.z];

        if (gameObjects == null)
        {
            grid[cellPos.x, cellPos.y, cellPos.z] = new List<GameObject>();
        }
        Debug.Log("Save gameobject " + go.name + " with world pos " + pos + " to cell position " + cellPos);
        grid[cellPos.x, cellPos.y, cellPos.z].Add(go);
    }

    public bool HasGameObjects(Vector3 worldPos)
    {
        if (!GameManager.Instance.isValidPos(worldPos))
        {
            throw new System.Exception("GameObject is out of bounds");
        }

        Vector3Int cellPos = ConvertWorldPosToGridPos(worldPos);

        return grid[cellPos.x, cellPos.y, cellPos.z] != null;
    }

    public List<GameObject> GetGameObjects(Vector3 worldPos)
    {
        if (!GameManager.Instance.isValidPos(worldPos))
        {
            throw new System.Exception("GameObject is out of bounds " + worldPos);
        }

        Vector3Int cellPos = ConvertWorldPosToGridPos(worldPos);

        Debug.Log("Get GameObjects at world position " + worldPos + " and cell position " + cellPos);


        return grid[cellPos.x, cellPos.y, cellPos.z];
    }
}
