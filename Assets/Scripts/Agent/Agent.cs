using UnityEngine;
using System.Collections;
using System;

public class Agent : MonoBehaviour
{
    Coroutine MoveRoutine;

    float rotationSpeed = 1;
    float movementSpeed = 1;
    float continuousRotateSpeed = 10; // complete this rotation faster because it is an interrupting rotation

    public float rotationThreshold = 0.1f;
    public float distanceThreshold = 0.1f;

    Vector3 lookRotation = DefaultUtils.DefaultVector3;
    bool isLookInProgress; // a look is in progress that must complete before any other looks can be processed
    protected bool isMoveInProgress; // a move is in progress that must complete before any other movements can be processed

    public static event Action<Transform, Vector3> OnDig;

    Vector3 prevPosition = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

    public bool isDigging;

    Quaternion targetRotation;
    Vector3 targetPosition;

    Vector3 startNotifyPosition;

    public Segment curSegment;
    public Vector3 curSegmentForward; // the direction of the tunnel the player is facing

    // Use this for initialization
    protected virtual void Start()
	{
        startNotifyPosition = transform.position;
        isLookInProgress = false;
        isMoveInProgress = false;
        curSegmentForward = DefaultUtils.DefaultVector3;
    }

    // Update is called once per frame
    protected virtual void Update()
	{
        if (isLookInProgress)
        {
            isLookInProgress = Rotate(lookRotation, continuousRotateSpeed);
        }
        // check if player's rotation changes based on direction within tunnel
        else if (curSegment != null && !DirectionUtils.isDirectionsAligned(transform.forward, curSegmentForward)) // for ex. if player has turned around in the current tunnel
        {
            // if traveling to the other end of the segment, update the segment forward vector
            curSegmentForward = -curSegmentForward;
            float xRot = DirectionUtils.GetUpDownRotation(transform.forward, curSegment.forward);
            Vector3 verticalRotate = new Vector3(xRot, transform.eulerAngles.y, transform.eulerAngles.z);
            ChangeRotation(verticalRotate, true);
        }
    }

    IEnumerator MoveToDestination(Vector3 targetPosition, float speed)
    {
        Debug.Log("Move to destination " + targetPosition);
        float startTime = Time.time;
        Vector3 startPosition = transform.position;

        while (Time.time - startTime < 1.0f / speed)
        {
            float t = (Time.time - startTime) * speed;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        isMoveInProgress = false;
        // Ensure the object reaches the exact destination
        transform.position = targetPosition;
    }

    public void AbortMovement()
    {
        isMoveInProgress = false;
        StopCoroutine(MoveRoutine);
    }

    /// <summary>
    /// Update segment player is in
    /// </summary>
    /// <param name="segment">segment of tunnel</param>
    public void UpdateSegment(Segment segment)
    {
        curSegment = segment;
        curSegmentForward = DirectionUtils.isDirectionsAligned(transform.forward, curSegment.forward) ? segment.forward : -segment.forward;
    }

    bool Rotate(Vector3 rotation, float rotateSpeed)
    {
        // Create a rotation that points in the specified direction
        Quaternion targetRotation = Quaternion.Euler(rotation);

        // Check if the current rotation is close enough to the target rotation
        bool isInProgress = Quaternion.Angle(transform.rotation, targetRotation) > rotationThreshold;

        if (isInProgress)
        {
            // Interpolate between the current rotation and the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }

        return isInProgress;
    }

    /// <summary>
    /// Move in a direction
    /// </summary>
    /// <param name="destination">final destination</param>
    /// <param name="isContinuous">Will this movement happen over multiple frames</param>
    public void ChangeMovement(Vector3 destination, bool isContinuous, float speed)
    {
        if (isMoveInProgress)
        {
            return;
        }
        if (isContinuous)
        {
            isMoveInProgress = true;
            MoveRoutine = StartCoroutine(MoveToDestination(destination, speed));
        }
        else
        {
            transform.position = destination;
        }
    }

    /// <summary>
    /// Change the target rotation
    /// </summary>
    /// <param name="targetRotation">the target rotation that sets rotation along the non-zero axes</param>
    /// <param name="isContinuous">Will this rotation happen over multiple frames</param>
    public void ChangeRotation(Vector3 targetRotation, bool isContinuous)
    {
        if (isLookInProgress)
        {
            Debug.Log("Look Rotation blocked because another look is in progress");
            return;
        }

        lookRotation = targetRotation;

        //Debug.Log("Look Rotation change to " + targetRotation);

        if (isContinuous) // a continuous rotation happens over multiple frames, until it is complete we set the block flag to true
        {
            isLookInProgress = true;
        }
        else
        {
            Rotate(lookRotation, rotationSpeed); // do a rotation in a single frame
        }
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

