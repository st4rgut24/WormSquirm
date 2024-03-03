using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : CharacterAnimator
{
    static string[] animNames = { Consts.SwingAnim, Consts.ThrowAnim, Consts.ShootAnim, Consts.ChainAnim };

    static Dictionary<ToolType, string> WeaponAnimPairs = new Dictionary<ToolType, string>
    {
        { ToolType.Pickaxe, Consts.SwingAnim },
        { ToolType.Crossbow, Consts.ShootAnim },
        { ToolType.Chain, Consts.ChainAnim }
    };

    public void TriggerWeaponAnimation(ToolType type)
    {
        string weaponAnimName = WeaponAnimPairs[type];
        TriggerAnimation(weaponAnimName);
    }

    public PlayerAnimator(Animator animator) : base(animator, animNames)
    {
	}
}

