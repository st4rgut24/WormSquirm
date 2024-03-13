using System;
using System.Collections.Generic;
using UnityEngine;

public class Pickaxe : Melee
{
    public static event Action<Vector3> Dig;
    
    Vector2 digDirection = DefaultUtils.DefaultVector3;

    private void OnEnable()
    {
        MainPlayer.MeleeAttackEvent += OnAttack;
    }

    private void Awake()
    {
        toolType = ToolType.Pickaxe;
        damage = 100;
    }

    public void OnAttack()
    {
        // Dig, if the attack was not directed toward another agent
        if (attackState != AttackState.Attack)
        {
            Vector3 direction = GetDirection();
            Dig?.Invoke(direction);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        MainPlayer.MeleeAttackEvent -= OnAttack;
    }
}

