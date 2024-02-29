using System;
using UnityEngine;
using System.Collections;
using System.Linq;

public class Player : Agent
{
    public delegate void ChangeSideRotation(float horRot, float? speed);
    public delegate void ChangeMoveDelegate(Vector3 destination, bool isContinuous, float speed);

    public const string swingAnimName = "isSwinging";
    public const string throwAnimName = "isThrowing";

    string[] animNames = { swingAnimName, throwAnimName };

    protected override void Start()
    {
        // Start the coroutine
        base.Start();

        string[] allAnimNames = animNames.Concat(agentAnimNames).ToArray();
        charAnimator = new CharacterAnimator(animator, allAnimNames);
    }

    protected override void Update()
    {
        base.Update();
    }
}