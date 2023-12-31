using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GridType
{
    Tunnel
}

public class GridManager
{
    Dictionary<GridType, Grid> GridDictionary;

    public GridManager()
    {
        GridDictionary = new Dictionary<GridType, Grid>();

        GridDictionary.Add(GridType.Tunnel, new Grid(4, 3));
    }

    public Grid GetGrid(GridType gridType)
    {
        if (GridDictionary.ContainsKey(gridType))
        {
            return GridDictionary[gridType];
        }
        else
        {
            throw new System.Exception("Not a valid grid type: " + gridType);
        }
    }
}

