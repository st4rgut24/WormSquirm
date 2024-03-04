using System;
using System.Collections.Generic;
using UnityEngine;

public class Pickaxe : Melee
{
    public static event Action<Vector3> Dig;
    
    Vector2 digDirection = DefaultUtils.DefaultVector3;

    private void Awake()
    {
        toolType = ToolType.Pickaxe;
        damage = 100;
    }

    public override void Use()
    {
        base.Use();
        Vector3 direction = GetDirection();
        Dig?.Invoke(direction);
    }
}

