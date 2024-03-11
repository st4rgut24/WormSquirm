using System;
using UnityEngine;
using System.Collections;
using System.Linq;

public class Player : Agent
{
    public delegate void ChangeSideRotation(float horRot, float? speed);
    public delegate void ChangeMoveDelegate(Vector3 destination, bool isContinuous, float speed);

    protected PlayerAnimator playerAnimator;

    protected override void Start()
    {
        // Start the coroutine
        base.Start();

        //string[] allAnimNames = animNames.Concat(agentAnimNames).ToArray();
        playerAnimator = new PlayerAnimator(animator);
        charAnimator = playerAnimator;
    }

    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Inflict damage on opponent if in proximity
    /// </summary>
    /// <param name="damage">amount of damage to inflict</param>
    /// <param name="attackedAgent">agent that is attacked</param>
    protected override void InflictDamage(float damage, Agent attackedAgent)
    {
        base.InflictDamage(damage, attackedAgent);
    }

    protected override IEnumerator DieCoroutine()
    {
        charAnimator.TriggerAnimation(Consts.DieAnim);
        yield return null;
        // TODO: maybe emit an event to trigger some end screen
    }
}