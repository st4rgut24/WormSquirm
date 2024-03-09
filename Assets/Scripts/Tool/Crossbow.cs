using UnityEngine;
using System.Collections;

public class Crossbow : Ranged
{
    private void Awake()
    {
        toolType = ToolType.Crossbow;
        damage = 100;
        animShootFrame = 12;
        animator = GetComponent<Animator>();
    }
}

