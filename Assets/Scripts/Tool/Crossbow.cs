using UnityEngine;
using System.Collections;

public class Crossbow : Ranged
{
    private void Awake()
    {
        toolType = ToolType.Crossbow;
    }
}

