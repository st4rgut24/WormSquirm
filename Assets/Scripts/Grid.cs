using UnityEngine;
using System.Collections.Generic;

public class Grid
{
    Vector3Int dimension;

    public int unitsPerCell; 

    int searchBuffer;        // number of adjoining cells need to check due to uncertainty
                             // of gameobject size

    private List<GameObject>[,,] grid;

    /// <summary>
    /// Create a grid to lookup GameObjects by position
    /// </summary>
    /// <param name="unitsPerCell"> world units per array indice </param>
    /// <param name="maxDimSize"> max size (l,w,h) of the stored gameobjects </param>
    public Grid(int unitsPerCell, int maxDimSize) {

        this.unitsPerCell = unitsPerCell;
        this.searchBuffer = getSearchBuffer(maxDimSize, unitsPerCell);

        this.dimension = getDimension();
        grid = new List<GameObject>[this.dimension.x, this.dimension.y, this.dimension.z];
    }


    int getSearchBuffer(int maxDimSize, int unitsPerCell)
    {
        return Mathf.FloorToInt(maxDimSize / this.unitsPerCell) + 1;
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
        // Debug.Log("Save gameobject " + go.name + " with world pos " + pos + " to cell position " + cellPos);
        grid[cellPos.x, cellPos.y, cellPos.z].Add(go);
    }

    //public List<GameObject> GetGameObjects(Vector3 worldPos)
    //{
    //    if (!GameManager.Instance.isValidPos(worldPos))
    //    {
    //        throw new System.Exception("GameObject is out of bounds " + worldPos);
    //    }

    //    Vector3Int cellPos = ConvertWorldPosToGridPos(worldPos);

    //    Debug.Log("Get GameObjects at world position " + worldPos + " and cell position " + cellPos);


    //    return grid[cellPos.x, cellPos.y, cellPos.z];
    //}

    bool isWithinGrid(int i, int j, int k)
    {
        return i >= 0 && i < grid.GetLength(0) &&
                        j >= 0 && j < grid.GetLength(1) &&
                        k >= 0 && k < grid.GetLength(2);
    }

    public List<GameObject> GetGameObjects(Vector3 worldPos)
    {
        if (!GameManager.Instance.isValidPos(worldPos))
        {
            throw new System.Exception("GameObject is out of bounds " + worldPos);
        }

        List<GameObject> result = new List<GameObject>();

        Vector3Int cellPos = ConvertWorldPosToGridPos(worldPos);

        // Debug.Log("Get GameObjects at cell position " + cellPos);

        int x = cellPos.x;
        int y = cellPos.y;
        int z = cellPos.z;

        // Iterate through the cells in the specified buffer around the indexed cell
        for (int i = x - this.searchBuffer; i <= x + this.searchBuffer; i++)
        {
            for (int j = y - this.searchBuffer; j <= y + this.searchBuffer; j++)
            {
                for (int k = z - this.searchBuffer; k <= z + this.searchBuffer; k++)
                {
                    if (isWithinGrid(i,j,k) && grid[i, j, k] != null)
                    {
                        // Add all objects from the current cell to the result list
                        result.AddRange(grid[i, j, k]);
                    }
                }
            }
        }

        return result;
    }
}
