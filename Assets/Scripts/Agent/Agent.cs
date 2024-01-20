using UnityEngine;
using System.Collections;
using System;
using static UnityEngine.GraphicsBuffer;

public class Agent : MonoBehaviour
{
    float rotationSpeed = 1;
    public float rotationThreshold = 0.1f;

    Vector3 lookRotation = DefaultUtils.DefaultVector3;
    //bool isLookComplete = true;
    bool isLookInProgress = true; // a look is in progress that must complete before any other looks can be processed

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
        isLookInProgress = false;
    }

    // Update is called once per frame
    protected virtual void Update()
	{
        if (isLookInProgress)
        {
            isLookInProgress = Rotate(lookRotation);

        }
        //if (IsLooking(lookRotation))
        //{
        //    Debug.Log("Look in direction " + lookRotation);
        //    SmoothLookAtDirection(lookRotation);
        //}
    }

    /// <summary>
    /// Determine whether player has finished rotation to look in new direction
    /// </summary>
    /// <param name="direction">new look direction</param>
    /// <returns>true if done looking in direction</returns>
    //bool IsLooking(Vector3 direction)
    //{
    //    return !direction.Equals(DefaultUtils.DefaultVector3) && !isLookComplete;
    //}

    bool Rotate(Vector3 rotation)
    {
        // Create a rotation that points in the specified direction
        Quaternion targetRotation = Quaternion.Euler(rotation);

        // Check if the current rotation is close enough to the target rotation
        bool isInProgress = Quaternion.Angle(transform.rotation, targetRotation) > rotationThreshold;

        if (isInProgress)
        {
            // Interpolate between the current rotation and the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        return isInProgress;
        //else
        //{
        //    //isLookComplete = true; // finished rotation
        //    isBlockingLook = false;
        //}
    }

    //void SmoothLookAtDirection(Vector3 direction)
    //{
    //    // Create a rotation that points in the specified direction
    //    Quaternion tunnelDirection = Quaternion.LookRotation(direction);
    //    Quaternion targetRotation = Quaternion.Euler(tunnelDirection.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);

    //    // Check if the current rotation is close enough to the target rotation
    //    if (Quaternion.Angle(transform.rotation, targetRotation) > rotationThreshold)
    //    {
    //        // Interpolate between the current rotation and the target rotation
    //        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    //    }
    //    else
    //    {
    //        isLookComplete = true; // finished rotation
    //    }
    //}

    /// <summary>
    /// Change the target rotation
    /// </summary>
    /// <param name="rotation">the target rotation that sets rotation along the non-zero axes</param>
    /// <param name="isContinuous">Will this rotation happen over multiple frames</param>
    public void ChangeRotation(Vector3 rotation, bool isContinuous)
    {
        if (isLookInProgress)
        {
            Debug.Log("Look Rotation blocked because another look is in progress");
            return;
        }

        Vector3 targetRotation = SetLookRotation(rotation);

        Debug.Log("Look Rotation change to " + targetRotation);

        if (isContinuous) // a continuous rotation happens over multiple frames, until it is complete we set the block flag to true
        {
            isLookInProgress = true;
        }
        else
        {
            Rotate(lookRotation); // do a rotation in a single frame
        }
    }

    /// <summary>
    /// Set the target rotation
    /// </summary>
    /// <param name="rotation">the non-zero values indicate target angles for respectives axes</param>
    public Vector3 SetLookRotation(Vector3 rotation)
    {
        lookRotation = transform.eulerAngles;

        if (rotation.x != 0)
        {
            lookRotation.x = rotation.x;
        }
        if (rotation.y != 0)
        {
            lookRotation.y = rotation.y;
        }
        if (rotation.z != 0)
        {
            lookRotation.z = rotation.z;
        }

        return lookRotation;
    }

    ///// <summary>
    ///// A change in the terrain causes a change in the direction agent is facing
    ///// </summary>
    ///// <param name="tunnelVector">forward vector of tunnel segment</param>
    //public void ChangeDirection(Vector3 tunnelVector)
    //{
    //    lookDirection = tunnelVector;
    //    isLookComplete = false;
    //}

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

