using System;
using UnityEngine;
using System.Collections;

public class Player : Agent
{
    public delegate void ChangeRotationDelegate(Vector3 rotation, bool isContinuous);

    protected override void Start()
    {
        // Start the coroutine
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
}