using System;
using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the Weapon Selector slider attached to the main player
/// </summary>
public class ToolSlider
{
	SimpleScrollSnap scrollSnap;
    Dictionary<ToolType, GameObject> WeaponUIPrefabDict;

    public ToolType[] InitialWeapons = { ToolType.Pickaxe, ToolType.Crossbow };

    public ToolSlider(SimpleScrollSnap scrollSnap, Dictionary<ToolType, GameObject> WeaponUIPrefabDict)
	{
        this.scrollSnap = scrollSnap;
        this.WeaponUIPrefabDict = WeaponUIPrefabDict;

        InitTools();
    }

    public void InitTools()
    {
        foreach (ToolType toolType in InitialWeapons)
        {
            GameObject WeaponUI = this.WeaponUIPrefabDict[toolType];
            AddTool(WeaponUI);
        }
    }

    public void RemoveTool(GameObject tool)
	{
		scrollSnap.RemoveObject(tool);
    }

	public void AddTool(GameObject tool)
	{
        scrollSnap.AddToBack(tool);
    }
}

