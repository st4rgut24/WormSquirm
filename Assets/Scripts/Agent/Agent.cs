using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Live agents have controlled movements, hence the functions for controlling rotation, etc.
/// </summary>
public abstract class Agent : MonoBehaviour
{
    public Segment curSegment;

    protected BoxCollider agentCollider;

    public Animator animator;
    public CharacterAnimator charAnimator;

    protected AgentHealth health;

    protected Coroutine MoveRoutine;
    protected bool isMoveInProgress; // a move is in progress that must complete before any other movements can be processed

    protected bool hasDied = false; //if the agent has died it should not receive any further commands

    public const string moveAnimName = "speed";
    public const string dieAnimName = "isDying";
        
    protected float height;

    protected string[] agentAnimNames = { dieAnimName, moveAnimName };

    public float rotationSpeed;

    public float rotationThreshold = 0.1f;
    public float distanceThreshold = 0.1f;

    protected Quaternion targetRotation;

    public abstract IEnumerator DieCoroutine();

    public static event Action<Transform, Vector3> OnDig;

    Vector3 prevPosition = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

    public bool isDigging;

    Vector3 targetPosition;

    Vector3 startNotifyPosition;

    public Vector3 curSegmentForward; // the direction of the tunnel the player is facing

    // Use this for initialization
    protected virtual void Start()
	{
        startNotifyPosition = transform.position;
        isMoveInProgress = false;
        curSegmentForward = DefaultUtils.DefaultVector3;
        animator = GetComponent<Animator>();
        agentCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    protected virtual void Update()
	{
        //transform.rotation = targetRotation; // more appropriate for rock rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        // check if player's rotation changes based on direction within tunnel

        if (curSegment != null && !DirectionUtils.isDirectionsAligned(transform.forward, curSegmentForward)) // for ex. if player has turned around in the current tunnel
        {
            // if traveling to the other end of the segment, update the segment forward vector
            curSegmentForward = -curSegmentForward;
            float xRot = DirectionUtils.GetUpDownRotation(transform.forward, curSegment.forward);
            //Vector3 verticalRotate = new Vector3(xRot, transform.eulerAngles.y, transform.eulerAngles.z);
            // Rotations happen over several frames until playerr reaches target destination
            ChangeVerticalRotation(xRot, Consts.defaultRotationSpeed);
        }
    }

    /// <summary>
    /// Make an agent fall to the level of the ground of the tunnel segment
    /// </summary>
    protected void Fall()
    {
        Vector3 fallPos = transform.position - transform.up.normalized * height;
        Debug.Log("fall pos is " + fallPos);
        //transform.position = fallPos;
        ChangeMovement(fallPos, true, Consts.FallSpeed);
    }

    protected void DisableCollider()
    {
        agentCollider.enabled = false;
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

        if (charAnimator != null)
        {
            charAnimator.SetAnimationMovement(speed);
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

    public Vector3 GetClosestCenterPoint()
    {
        return curSegment.GetClosestPointToCenterline(transform.position);
    }

    /// <summary>
    /// Is the player traveling in a direction that is going out of the segment bounds
    /// </summary>
    /// <param name="position">Player transform</param>
    /// <param name="originalPosition">player's original position</param>
    /// <param name="projectedPosition">player's next position</param>
    /// <returns>true if out of bounds</returns>
    public bool isGoingOutOfBounds(Transform transform, Vector3 originalPosition, Vector3 projectedPosition)
    {
        bool outOfBounds = curSegment.IsOutOfBounds(transform, originalPosition, projectedPosition);
        return outOfBounds;
    }

    protected IEnumerator MoveToDestination(Vector3 targetPosition, float speed)
    {
        float startTime = Time.time;
        Vector3 startPosition = transform.position;

        if (charAnimator != null)
        {
            charAnimator.SetAnimationMovement(speed);
        }

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

    public virtual bool TakeDamage(float damage)
    {
        if (!hasDied) {
            hasDied = health.ReduceHealth(damage);

            if (hasDied)
            {
                StartCoroutine(DieCoroutine());
            }
        }

        return hasDied;
    }

    protected virtual void InflictDamage(float damage, Agent attackedAgent)
    {
        attackedAgent.TakeDamage(damage);
    }

    public void AbortMovement()
    {
        isMoveInProgress = false;

        if (MoveRoutine != null)
        {
            StopCoroutine(MoveRoutine);
        }

        if (charAnimator != null)
        {
            charAnimator.SetAnimationMovement(0);
        }
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
        // Debug.Log("Change vertical rotation to " + vertRot + " at speed " + rotateSpeed);
        Vector3 rot = new Vector3(vertRot, this.targetRotation.eulerAngles.y, this.targetRotation.eulerAngles.z);
        ChangeRotation(rot, rotateSpeed);
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

    protected void notifyDig(Vector3 digDirection)
    {
        if (transform.position != prevPosition)
        {
            // Debug.Log("Notify dig. Current position " + transform.position + " Previous position " + prevPosition);
            prevPosition = transform.position;
            OnDig?.Invoke(transform, digDirection);
        }
    }
}

