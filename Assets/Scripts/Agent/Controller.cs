using System;
using UnityEngine;

public struct Control {
    float magnitude;
    Vector2 direction;
}

public class Controller
{
    Transform transform;

    public float rotationSpeed = .1f;
    public float acceleration = 5;
    public float deceleration = 2f; // Adjust the deceleration factor
    private float currentSpeed = 0f;
    private float maxSpeed = 5f;

    public float movementThreshold = 1; // the minimum magnitude of vector to move a player

    public Controller(Transform transform)
    {
        this.transform = transform;
    }

    public void AccelerateInCurrentDirection()
    {
        // Get the forward direction of the player in world space
        if (currentSpeed < maxSpeed)
        {
            Vector3 forwardDirection = transform.forward;

            // Accelerate the player in the current direction
            currentSpeed += acceleration * Time.deltaTime;
            float moveDistance = currentSpeed * Time.deltaTime;
            transform.Translate(forwardDirection * moveDistance, Space.World);
        }
        else {
            Decelerate();
        }
    }

    void Decelerate()
    {
        Debug.Log("Decelerate");
        // Decelerate the player when the spacebar is not pressed
        currentSpeed -= deceleration * Time.deltaTime;
        currentSpeed = Mathf.Max(currentSpeed, 0f); // Ensure speed doesn't go below zero
        float moveDistance = currentSpeed * Time.deltaTime;

        // If the player is moving, translate the player to simulate deceleration
        if (currentSpeed > 0f)
        {
            transform.Translate(transform.forward * moveDistance, Space.World);

            // Notify subscribers about the move event
        }
    }

    /// <summary>
    /// Handle input via controls
    /// </summary>
    /// <param name="rawInput">unnormalized input</param>
    public void HandleInput(Vector2 rawInput) {
        if (rawInput.Equals(DefaultUtils.DefaultVector3))
        {
            Decelerate();
        }
        else {
            Move(rawInput);
            Rotate(rawInput);
        }
    }

    /// <summary>
    /// Move the player forward
    /// </summary>
    /// <param name="inputMovement">vector representing movement input</param>
    public void Move(Vector2 inputMovement)
    {
        if (inputMovement.magnitude > movementThreshold)
        {
            AccelerateInCurrentDirection();
        }
        else
        {
            Decelerate();
        }
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
        Quaternion rotation = Quaternion.AngleAxis(angle, transform.up);
        // TODO: Adjust rotation speed based on angle size
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
}

