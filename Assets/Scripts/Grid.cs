using UnityEngine;
using System.Collections.Generic;

public class Grid
{
    Vector3Int dimension;

    public int unitsPerCell; 

    const int searchBuffer = 3;        // number of adjoining cells need to check due to uncertainty
                             // of gameobject size

    private List<GameObject>[,,] grid;

    /// <summary>
    /// Create a grid to lookup GameObjects by position
    /// </summary>
    /// <param name="unitsPerCell"> world units per array indice </param>
    /// <param name="maxDimSize"> max size (l,w,h) of the stored gameobjects </param>
    public Grid(int unitsPerCell, int maxDimSize) {

        this.unitsPerCell = unitsPerCell;

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

    bool isWithinGrid(int x, int y, int z)
    {
        return x >= 0 && x < grid.GetLength(0) &&
                        y >= 0 && y < grid.GetLength(1) &&
                        z >= 0 && z < grid.GetLength(2);
    }

    /// <summary>
    /// Get GameObjects surrounding position
    /// </summary>
    /// <param name="worldPos">position</param>
    /// <param name="searchMultiplier">factor to expand the search radius</param>
    /// <returns>gameobjects within the search area</returns>
    /// <exception cref="System.Exception">invalid position exception</exception>
    public List<GameObject> GetGameObjects(Vector3 worldPos, int searchMultiplier)
    {
        if (!GameManager.Instance.isValidPos(worldPos))
        {
            throw new System.Exception("GameObject is out of bounds " + worldPos);
        }

        List<GameObject> result = new List<GameObject>();
         
        Vector3Int cellPos = ConvertWorldPosToGridPos(worldPos);

        Debug.Log("Get GameObjects at cell position " + cellPos + " world Pos " + worldPos);

        int x = cellPos.x;
        int y = cellPos.y;
        int z = cellPos.z;

        int buffer = searchBuffer * searchMultiplier;

        // Iterate through the cells in the specified buffer around the indexed cell
        for (int i = x - buffer; i <= x + buffer; i++)
        {
            for (int j = y - buffer; j <= y + buffer; j++)
            {
                for (int k = z - buffer; k <= z + buffer; k++)
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
