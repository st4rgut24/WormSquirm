using System;
using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the Weapon Selector slider attached to the main player
/// </summary>
public class ToolSlider : Slider
{
    public int[] InitialWeapons = { (int)ToolType.Pickaxe, (int)ToolType.Crossbow, (int)ToolType.Mace };

    public ToolSlider(SimpleScrollSnap scrollSnap, Dictionary<int, GameObject> WeaponUIPrefabDict) : base(scrollSnap, WeaponUIPrefabDict)
	{
        InitObjects(InitialWeapons);
    }
}

