using UnityEngine;
using System.Collections;
using System;
using static UnityEngine.GraphicsBuffer;

public class Agent : MonoBehaviour
{
    float rotationSpeed = 5;
    public float rotationThreshold = 0.1f;

    Vector3 lookDirection = DefaultUtils.DefaultVector3;
    bool isLookComplete = true;

    public static event Action<Transform, Vector3> OnDig;

    Vector3 prevPosition = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

    public bool isDigging;

    Quaternion targetRotation;
    Vector3 targetPosition;

    Vector3 startNotifyPosition;

    // Use this for initialization
    protected virtual void Start()
	{
        startNotifyPosition = transform.position;
    }

    // Update is called once per frame
    protected virtual void Update()
	{
        if (IsLooking(lookDirection))
        {
            SmoothLookAtDirection(lookDirection);
        }
    }

    /// <summary>
    /// Determine whether player has finished rotation to look in new direction
    /// </summary>
    /// <param name="direction">new look direction</param>
    /// <returns>true if done looking in direction</returns>
    bool IsLooking(Vector3 direction)
    {
        return !direction.Equals(DefaultUtils.DefaultVector3) && !isLookComplete;
    }

    void SmoothLookAtDirection(Vector3 direction)
    {
        // Create a rotation that points in the specified direction
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Check if the current rotation is close enough to the target rotation
        if (Quaternion.Angle(transform.rotation, targetRotation) > rotationThreshold)
        {
            // Interpolate between the current rotation and the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            isLookComplete = true; // finished rotation
        }
    }

    /// <summary>
    /// A change in the terrain causes a change in the direction agent is facing
    /// </summary>
    public void ChangeDirection(Vector3 forwardVector)
    {
        lookDirection = forwardVector;
        isLookComplete = false;
    }

    protected void notifyDig(Vector3 digDirection)
    {
        if (transform.position != prevPosition)
        {
            Debug.Log("Notify dig. Current position " + transform.position + " Previous position " + prevPosition);
            prevPosition = transform.position;
            OnDig?.Invoke(transform, digDirection);
        }
    }
}

