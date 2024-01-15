using System;
using UnityEngine;
using System.Collections;

public class Player : Agent
{
    //public float rotationSpeed = 50f;
    //public float acceleration = 5f;
    //public float deceleration = 2f; // Adjust the deceleration factor
    //private float currentSpeed = 0f;
        

    protected override void Start()
    {
        // Start the coroutine
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        //// Get input from arrow keys
        //float horizontalInput = Input.GetAxis("Horizontal");
        //float verticalInput = Input.GetAxis("Vertical");
        //float upDownInput = Input.GetAxis("UpDown");

        //// Calculate rotation angles
        //float yaw = horizontalInput * rotationSpeed * Time.deltaTime;
        //float pitch = upDownInput * rotationSpeed * Time.deltaTime;
        //float roll = verticalInput * rotationSpeed * Time.deltaTime;

        //// Rotate the player
        //transform.Rotate(new Vector3(pitch, yaw, roll));

        //// Check if spacebar is pressed
        //if (Input.GetKey(KeyCode.Space))
        //{
        //    // Accelerate the player in the current direction
        //    AccelerateInCurrentDirection();
        //}
        //else
        //{
        //    // Decelerate the player when the spacebar is not pressed
        //    Decelerate();
        //}
    }

    //public void AccelerateInCurrentDirection()
    //{
    //    // Get the forward direction of the player in world space
    //    Vector3 forwardDirection = transform.forward;

    //    // Accelerate the player in the current direction
    //    currentSpeed += acceleration * Time.deltaTime;
    //    float moveDistance = currentSpeed * Time.deltaTime;
    //    transform.Translate(forwardDirection * moveDistance, Space.World);
    //}

    //void Decelerate()
    //{
    //    // Decelerate the player when the spacebar is not pressed
    //    currentSpeed -= deceleration * Time.deltaTime;
    //    currentSpeed = Mathf.Max(currentSpeed, 0f); // Ensure speed doesn't go below zero
    //    float moveDistance = currentSpeed * Time.deltaTime;

    //    // If the player is moving, translate the player to simulate deceleration
    //    if (currentSpeed > 0f)
    //    {
    //        transform.Translate(transform.forward * moveDistance, Space.World);

    //        // Notify subscribers about the move event
    //    }
    //}
}