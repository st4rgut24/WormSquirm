using System;
using UnityEngine;

public class Animation
{
    Animator animator;
	AnimationClip clipInfo;
	float clipSpeed; // original clip speed

	public Animation(Animator animator)
	{
        this.animator = animator;
        this.clipInfo = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        this.clipSpeed = animator.speed;
	}

    public void Pause(int targetFrame)
    {
        float normalizedTime = targetFrame / clipInfo.length;

        animator.Play(clipInfo.name, 0, normalizedTime);
        animator.speed = 0;
    }

    public void Play()
    {
        animator.speed = clipSpeed;
    }
}