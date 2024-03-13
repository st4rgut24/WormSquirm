using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : CharacterAnimator
{
    static string[] animNames = { Consts.SwingAnim, Consts.ThrowAnim, Consts.ShootAnim, Consts.SnipeAnim, Consts.ChainAnim };

    static Dictionary<ToolType, Anim> WeaponAnimPairs = new Dictionary<ToolType, Anim>
    {
        { ToolType.Pickaxe, new Anim(Consts.SwingAnim, ActivateType.SetTrigger) },
        { ToolType.Mace, new Anim(Consts.ChainAnim, ActivateType.SetTrigger) },
        { ToolType.Crossbow, new Anim(Consts.ShootAnim, ActivateType.SetBool) }
    };

    /// <summary>
    /// These animations are paused, and only play a single frame for aiming stability
    /// </summary>
    static Dictionary<ToolType, Anim> WeaponPauseAnimPairs = new Dictionary<ToolType, Anim>
    {
        { ToolType.Crossbow, new Anim(Consts.SnipeAnim, ActivateType.SetBool) }
    };

    /// <summary>
    /// Plays player animation when equipped with a weapon
    /// </summary>
    /// <param name="type">type of weapon equipped</param>
    /// <param name="isPaused">whether the animation is paused or normal</param>
    public void PlayWeaponAnimation(ToolType type, bool isPaused)
    {
        if (isPaused)
        {
            PlayWeaponAnimation(type, WeaponPauseAnimPairs);
        }
        else
        {
            PlayWeaponAnimation(type, WeaponAnimPairs);
        }
    }

    private void PlayWeaponAnimation(ToolType type, Dictionary<ToolType, Anim> WeaponAnimPairs)
    {
        Anim WeaponAnim = WeaponAnimPairs[type];

        if (WeaponAnim.activateType == ActivateType.SetTrigger)
        {
            TriggerAnimation(WeaponAnim.name);
        }
        else if (WeaponAnim.activateType == ActivateType.SetBool)
        {
            SetLoopAnimation(WeaponAnim.name, true);
        }
        else
        {
            throw new Exception("Not a valid activation type: " + WeaponAnim.activateType);
        }
    }

    public void StopWeaponAnimation(ToolType type, bool isPaused)
    {
        if (isPaused)
        {
            StopWeaponAnimation(type, WeaponPauseAnimPairs);
        }
        else
        {
            StopWeaponAnimation(type, WeaponAnimPairs);
        }
    }

    private void StopWeaponAnimation(ToolType type, Dictionary<ToolType, Anim> WeaponAnimPairs)
    {
        if (!WeaponAnimPairs.ContainsKey(type))
        {
            return; // this tool type does not map to any animation
        }

        Anim WeaponAnim = WeaponAnimPairs[type];

        if (WeaponAnim.activateType == ActivateType.SetBool)
        {
            SetLoopAnimation(WeaponAnim.name, false);
        }
    }

    public PlayerAnimator(Animator animator) : base(animator, animNames)
    {
	}
}

