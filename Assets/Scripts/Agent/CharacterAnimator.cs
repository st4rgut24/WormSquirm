using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator
{
	Animator animator;
    Dictionary<string, int> AnimNamesToIds = new Dictionary<string, int>();

    int speedAnimationId;

    /// <summary>
    /// is the current animation being played interruptible
    /// </summary>
    bool isCurrentAnimationInterruptible;

    public CharacterAnimator(Animator animator, string[] animationNames)
	{
        this.animator = animator;

		for (int i = 0; i < animationNames.Length; i++)
		{
            string name = animationNames[i];
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
            animator.SetFloat(speedAnimationId, speed);
        }
    }

    /// <summary>
    /// Play an animation by setting a trigger
    /// </summary>
    /// <param name="name">name of the animation</param>
    public void TriggerAnimation(string name)
    {
        int animId = AnimNamesToIds[name];
        animator.SetTrigger(animId);
    }
}

