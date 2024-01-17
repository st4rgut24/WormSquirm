﻿using UnityEngine;
using System.Collections;

public enum PlayerState
{
    Dig,
    Stuck,
    Walk
}

public class MainPlayer : Player
{
    PlayerState state;

    Controller controller;
    Joystick joystick;

    bool isPositionClamped;

    private void OnEnable()
    {
        Pickaxe.Dig += HandleDig;
    }

    protected override void Start()
    {        
        base.Start();

        state = PlayerState.Dig;

        GameObject joystickGo = GameObject.Find("Variable Joystick");
        joystick = joystickGo.GetComponent<Joystick>();

        controller = new Controller(transform);

        isPositionClamped = false;
    }

    protected override void Update()
    {
        base.Update();

        if (joystick.HasInput())
        {
            Vector2 rawInput = joystick.GetInput();
            controller.HandleInput(rawInput);
        }
    }

    /// <summary>
    /// Handle digging action
    /// </summary>
    /// <param name="direction">Direction to dig in</param>
    public void HandleDig(Vector3 direction)
    {
        state = PlayerState.Dig;
        Debug.Log("Start Digging");
        notifyDig(direction);

        //controller.AccelerateInDirection(direction);
    }

    private void OnDisable()
    {
        Pickaxe.Dig -= HandleDig;
    }
}
