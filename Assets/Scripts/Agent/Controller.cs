﻿using System;
using UnityEngine;

public struct Control
{
    float magnitude;
    Vector2 direction;
}

public class Controller
{
    Transform transform;

    public float rotationSpeed = .5f;
    public float acceleration = 1;
    public float deceleration = 2f; // Adjust the deceleration factor
    private float currentSpeed = 0f;
    private float maxSpeed = 5f;

    public float movementThreshold = 1; // the minimum magnitude of vector to move a player

    public Controller(Transform transform)
    {
        this.transform = transform;
    }

    /// <summary>
    /// Get translation vector after accelerating
    /// </summary>
    /// <param name="forwardDirection">forward facing vector</param>
    /// <returns>vector to translate player</returns>
    public Vector3 GetAccelerateTranslation()
    {
        // Accelerate the player in the current direction
        currentSpeed += acceleration * Time.deltaTime;
        float moveDistance = currentSpeed * Time.deltaTime;

        return transform.forward * moveDistance;
    }

    Vector3 GetDecelerateTranslation()
    {
        // Decelerate the player when the spacebar is not pressed
        currentSpeed -= deceleration * Time.deltaTime;
        currentSpeed = Mathf.Max(currentSpeed, 0f); // Ensure speed doesn't go below zero
        float moveDistance = currentSpeed * Time.deltaTime;

        // If the player is moving, translate the player to simulate deceleration
        if (currentSpeed > 0f)
        {
            return transform.forward * moveDistance;
        }
        else
        {
            return DefaultUtils.DefaultVector3;
        }
    }

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
        Vector3 translationVector = GetTranslation(rawInput);
            
        if (translationVector == DefaultUtils.DefaultVector3)
        {
            return;
        }

        Vector3 projectedPosition = transform.position + translationVector;

        // TODO: need to update the segment the player is in if moving into a new segment

        if (!ClampPosition(projectedPosition))
        {
            transform.Translate(translationVector, Space.World);
        }
        else // the position does not change because it has exceeded the bounds of the current segment
        {
            TunnelManager.Instance.UpdateTransformDict(transform);
        }
    }

    /// <summary>
    /// Get projected movement in the player's forward direction
    /// </summary>
    /// <param name="inputMovement">vector representing movement input</param>
    public Vector3 GetTranslation(Vector2 inputMovement)
    {
        if (currentSpeed < maxSpeed)
        {
            return GetAccelerateTranslation();
        }
        else
        {
            return GetDecelerateTranslation();
        }
    }

    /// <summary>   
    /// If user surpasses the bounds of current tunnel segment, don't allow any movement
    /// </summary>
    /// <returns>whether position is clamped</returns>
    public bool ClampPosition(Vector3 position)
    {
        return SegmentManager.Instance.IsSegmentBoundsExceeded(transform, position);
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
    /// <param name="inputDirection">direction of the player via controls</param>
    public void Rotate(Vector2 inputDirection)
    {
        float angle = GetAngleFromInput(inputDirection);
        Quaternion inputRotation = Quaternion.AngleAxis(angle, transform.up);
        Quaternion targetRotation = transform.rotation * inputRotation;

        // TODO: Adjust rotation speed based on angle size
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}