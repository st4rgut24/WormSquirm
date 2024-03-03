using UnityEngine;
using System.Collections;
using System;

public class Agent : MonoBehaviour
{
    public Animator animator;
    public CharacterAnimator charAnimator;

    protected AgentHealth health;
    BoxCollider collider;

    public const string moveAnimName = "speed";
    public const string dieAnimName = "isDying";

    protected string[] agentAnimNames = { dieAnimName, moveAnimName };

    Coroutine MoveRoutine;

    public float rotationSpeed;

    public float rotationThreshold = 0.1f;
    public float distanceThreshold = 0.1f;

    Quaternion targetRotation;

    protected bool isMoveInProgress; // a move is in progress that must complete before any other movements can be processed

    public static event Action<Transform, Vector3> OnDig;

    Vector3 prevPosition = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

    public bool isDigging;

    Vector3 targetPosition;

    Vector3 startNotifyPosition;

    public Segment curSegment;
    public Vector3 curSegmentForward; // the direction of the tunnel the player is facing

    // Use this for initialization
    protected virtual void Start()
	{
        startNotifyPosition = transform.position;
        isMoveInProgress = false;
        curSegmentForward = DefaultUtils.DefaultVector3;
        animator = GetComponent<Animator>();
        collider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    protected virtual void Update()
	{
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        // check if player's rotation changes based on direction within tunnel
        if (curSegment != null && !DirectionUtils.isDirectionsAligned(transform.forward, curSegmentForward)) // for ex. if player has turned around in the current tunnel
        {
            // if traveling to the other end of the segment, update the segment forward vector
            curSegmentForward = -curSegmentForward;
            float xRot = DirectionUtils.GetUpDownRotation(transform.forward, curSegment.forward);
            //Vector3 verticalRotate = new Vector3(xRot, transform.eulerAngles.y, transform.eulerAngles.z);
            // Rotations happen over several frames until playerr reaches target destination
            ChangeVerticalRotation(xRot, Consts.rotationSpeed);
        }
    }

    public void TakeDamage(float damage)
    {
        health.ReduceHealth(damage);
    }

    protected virtual void InflictDamage(float damage, Agent attackedAgent)
    {
        attackedAgent.TakeDamage(damage);
    }

    IEnumerator MoveToDestination(Vector3 targetPosition, float speed)
    {
        float startTime = Time.time;
        Vector3 startPosition = transform.position;

        charAnimator.SetAnimationMovement(speed);

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

        if (MoveRoutine != null)
        {
            StopCoroutine(MoveRoutine);
        }

        charAnimator.SetAnimationMovement(0);
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

    /// <summary>
    /// Rotate to the point, destination
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="rotateSpeed"></param>
    /// <returns></returns>
    public void ChangeVerticalRotation(float vertRot, float? rotateSpeed=null)
    {
        Debug.Log("Change vertical rotation to " + vertRot + " at speed " + rotateSpeed);
        Vector3 rot = new Vector3(vertRot, this.targetRotation.eulerAngles.y, this.targetRotation.eulerAngles.z);
        ChangeRotation(rot, rotateSpeed);
        // Check if the current rotation is close enough to the target rotation

        //bool isInProgress = Quaternion.Angle(transform.rotation, targetRotation) > rotationThreshold;

        //if (isInProgress)
        //{
        //    float speed = rotateSpeed.GetValueOrDefault(1 / Time.deltaTime); // default value results in instant rotation
        //    // Interpolate between the current rotation and the target rotation
        //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,speed * Time.deltaTime);
        //}

        //return isInProgress;
    }

    public void ChangeHorizontalRotation(float horRot, float? rotateSpeed=null)
    {
        Vector3 rot = new Vector3(this.targetRotation.eulerAngles.x, horRot, this.targetRotation.eulerAngles.z);
        ChangeRotation(rot, rotateSpeed);
    }

    public void ChangeRotation(Vector3 targetRotation, float? rotateSpeed = null)
    {
        this.targetRotation = Quaternion.Euler(targetRotation);
        this.rotationSpeed = rotateSpeed.GetValueOrDefault(1 / Time.deltaTime);
    }

    public void ChangeRotation(Quaternion targetRotation, float? rotateSpeed = null)
    {
        this.targetRotation = targetRotation;
        this.rotationSpeed = rotateSpeed.GetValueOrDefault(1 / Time.deltaTime);
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

        charAnimator.SetAnimationMovement(speed);
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

