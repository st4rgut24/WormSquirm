﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CharacterAnimator
{
    Animator animator;

    public enum ActivateType
    {
        SetBool,
        SetTrigger
    }

    public struct Anim
    {
        public string name;
        public ActivateType activateType; // if false it is a trigger animation

        public Anim(string name, ActivateType activateType)
        {
            this.name = name;
            this.activateType = activateType; // if false it is a trigger animation
        }
    }


protected string[] agentAnimNames = { Consts.DieAnim, Consts.MoveAnim };

    Dictionary<string, int> AnimNamesToIds = new Dictionary<string, int>();

    int speedAnimationId;

    /// <summary>
    /// is the current animation being played interruptible
    /// </summary>
    bool isCurrentAnimationInterruptible;

    public CharacterAnimator(Animator animator, string[] animationNames)
	{
        this.animator = animator;

        string[] allAnimNames = animationNames.Concat(agentAnimNames).ToArray();

        for (int i = 0; i < allAnimNames.Length; i++)
		{
            string name = allAnimNames[i];
            int id = Animator.StringToHash(name);
            AnimNamesToIds[name] = id;
        }

        speedAnimationId = AnimNamesToIds[Agent.moveAnimName];
    }

    /// <summary>
    /// Play movement animation only if player is not interrupting a non-movement animation
    /// </summary>
    /// <param name="speed"></param>
    public void SetAnimationMovement(float speed)
    {
        if (animator != null)
        {
            // Debug.Log("Set aniamtion movement for gameobject " + this.animator.gameObject.name + " to speed " + speed);
            animator.SetFloat(speedAnimationId, speed);
        }
    }

    public void Idle()
    {
        SetAnimationMovement(Consts.IdleSpeed);
    }

    /// <summary>
    /// Play an animation by setting a trigger
    /// </summary>
    /// <param name="name">name of the animation</param>
    public void TriggerAnimation(string name)
    {
        if (animator != null)
        {
            int animId = AnimNamesToIds[name];
            animator.SetTrigger(animId);
        }
    }

    public void SetLoopAnimation(string name, bool isLooping)
    {
        if (animator != null)
        {
            int animId = AnimNamesToIds[name];
            animator.SetBool(animId, isLooping);
        }
    }
}

