﻿using System;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;
using UnityEngine.UIElements;

public struct Control
{
    float magnitude;
    Vector2 direction;
}

public class Controller
{
    Transform transform;
    MainPlayer player;

    Player.ChangeSideRotation changeRotation;
    Player.ChangeMoveDelegate changeMovement;

    Vector3 lastInBoundsPosition;

    public float acceleration = 1;
    public float deceleration = 2f; // Adjust the deceleration factor
    private float currentSpeed = 0f;
    private float maxSpeed = 5f;

    public float movementThreshold = 1; // the minimum magnitude of vector to move a player
    public float rotationThreshold = 30; // minimum rotation in degrees to throttle the movement speed of player

    public Controller(GameObject playerGo, Player.ChangeSideRotation changeRotation, Player.ChangeMoveDelegate changeMovement)
    {
        this.transform = playerGo.transform;
        this.player = playerGo.GetComponent<MainPlayer>();
        this.changeRotation = changeRotation;
        this.changeMovement = changeMovement;
    }

    ///// <summary>
    ///// Get translation vector after accelerating
    ///// </summary>
    ///// <param name="forwardDirection">forward facing vector</param>
    ///// <returns>normalized vector to translate player</returns>
    //public Vector3 GetAccelerateTranslation()
    //{
    //    // Accelerate the player in the current direction
    //    currentSpeed += acceleration * Time.deltaTime;
    //    float moveDistance = currentSpeed * Time.deltaTime;

    //    return transform.forward * moveDistance;
    //    //return transform.forward * currentSpeed;
    //}

    //Vector3 GetDecelerateTranslation()
    //{
    //    // Decelerate the player when the spacebar is not pressed
    //    currentSpeed -= deceleration * Time.deltaTime;
    //    currentSpeed = Mathf.Max(currentSpeed, 0f); // Ensure speed doesn't go below zero
    //    float moveDistance = currentSpeed * Time.deltaTime;

    //    // If the player is moving, translate the player to simulate deceleration
    //    if (currentSpeed > 0f)
    //    {
    //        return transform.forward * moveDistance;
    //        //return transform.forward * currentSpeed;
    //    }
    //    else
    //    {
    //        return DefaultUtils.DefaultVector3;
    //    }
    //}

    /// <summary>
    /// Handle input via controls
    /// </summary>
    /// <param name="rawInput">unnormalized input</param>
    public void HandleInput(Vector2 rawInput)
    {

        Rotate(rawInput);
        Move(rawInput);
    }

    public void Move(Vector2 rawInput)
    {
        Vector3 forwardMovement = GetForwardMovement();
            
        if (forwardMovement == DefaultUtils.DefaultVector3)
        {
            return;
        }

        Vector3 projectedPosition = transform.position + forwardMovement;

        if (!player.isGoingOutOfBounds(transform, transform.position, projectedPosition))
        {
            changeMovement(projectedPosition, false, currentSpeed);
            lastInBoundsPosition = projectedPosition;
        }
        else
        {
            changeMovement(lastInBoundsPosition, false, 0);
            currentSpeed = 0;
        }

        TunnelManager.Instance.UpdateTransformDict(transform); // update whatever segment the player is in if necessary
    }

    /// <summary>
    /// Get projected movement in the player's forward direction
    /// </summary> movement input</param>
    public Vector3 GetForwardMovement()
    {
        // todo: check that our projection of player's next position is where the player actually will be
        currentSpeed = this.player.HasStamina() ? Consts.RunningSpeed : Consts.WalkingSpeed;
        float moveDistance = currentSpeed * Time.deltaTime;

        return transform.forward * moveDistance;
        //return transform.forward * currentSpeed;
    }

    /// <summary>
    /// Get angle of rotation using the joystick direction
    /// </summary>
    /// <param name="inputDirection">joystick direction</param>
    /// <returns>angle of rotation. positive is clockwise, negative counterclockwise</returns>
    public float GetAngleFromInput(Vector2 inputDirection)
    {
        Vector2 normalizedDirection = inputDirection.normalized;
        // Calculate the angle in radians using Mathf.Atan2
        float angleInRadians = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x);

        // Convert the angle from radians to degrees
        float angleInDegrees = Mathf.Rad2Deg * angleInRadians - 90;


        if (angleInDegrees < -180)
        {
            angleInDegrees += 360;
        }

        return -angleInDegrees;
    }

    /// <summary>
    /// Rotate the player in direction of input
    /// </summary>
    /// <param name="angle">angle of rotation</param>
    /// <returns>target rotation angle</returns>
    public void Rotate(Vector3 rawInput)
    {
        float angle = GetAngleFromInput(rawInput);
        float horRot = transform.eulerAngles.y + angle * .1f;
        //Vector3 sideToSideRotation = new Vector3(transform.eulerAngles.x, , transform.eulerAngles.z);
        changeRotation(horRot, Consts.rotationSpeed); 
    }
}
