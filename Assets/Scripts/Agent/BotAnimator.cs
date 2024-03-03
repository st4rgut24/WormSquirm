using System;
using UnityEngine;

public class BotAnimator : CharacterAnimator
{
    static string[] animNames = { Consts.SlamAttackAnim };

    public BotAnimator(Animator animator) : base(animator, animNames)
	{
	}
}

