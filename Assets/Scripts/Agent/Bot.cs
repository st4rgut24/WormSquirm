using System;
using System.Linq;
using UnityEngine;

public abstract class Bot : Agent
{
    public BotManager.BotType botType;

    public Transform objective;

    public float velocity; // Configurable velocity

    protected float startTime;

    protected bool reachedDestination;

    protected Route route;

    protected abstract void SetObjective();

    protected abstract bool IsReachedFinalDestination(Waypoint finalWP);

    protected abstract void ReachDestination();

    protected bool hasRoute;

    protected const string isAttackingAnimName = "isAttacking";

    string[] animNames = { isAttackingAnimName };

    protected virtual void Awake()
    {
        hasRoute = false;
        reachedDestination = true;
        SetObjective();
    }

    protected override void Start()
    {
        base.Start();

        string[] allAnimNames = animNames.Concat(agentAnimNames).ToArray();
        charAnimator = new CharacterAnimator(animator, allAnimNames);

        startTime = Time.time;

        Segment initSegment = this.route.GetInitSegment();

        if (initSegment != null)
        {
            AgentManager.Instance.InitTransformSegmentDict(transform, initSegment);
        }
    }

    public void initRoute(Route route)
    {
        if (!hasRoute)
        {
            hasRoute = true;
            transform.position = route.GetDestination();

            // initialize the direction bot is facing
            Waypoint CurWP = route.GetCurWaypoint();
            Waypoint NextWP = route.GetNextWaypoint();
            this.faceDirection(CurWP.position, NextWP.position);
        }

        this.route = route;
    }

    /// <summary>
    /// Face the direction of travel
    /// </summary>
    void faceDirection(Vector3 start, Vector3 dest)
    {
        Vector3 moveDirection = (dest - start).normalized;

        // Rotate the bot to face the direction it is moving in
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            Debug.Log("rotate bot " + transform.rotation);
        }
    }

    protected virtual void ReachWaypoint()
    {
        route.AdvanceWaypoint();
    }

    protected virtual void FixedUpdate()
    {
        Waypoint curWP = route.GetCurWaypoint();
        Waypoint finalWP = route.GetLastWaypoint();

        Debug.Log("Waypoint is " + curWP.position);

        if (IsReachedFinalDestination(finalWP))
        {
            charAnimator.TriggerAnimation(isAttackingAnimName);
            Debug.Log("Reached final waypoint");
            ReachDestination();
        }
        else if (transform.position == curWP.position) // reached destination
        {
            if (curWP.segment != null)
            {
                UpdateSegment(curWP.segment);
            }

            //Vector3 rayDir = miniRoute.end - miniRoute.start;
            //Debug.DrawRay(miniRoute.start, rayDir, Color.red);
            ReachWaypoint();
        }
        else
        {
            Move(curWP);
        }
    }

    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }

    private void Move(Waypoint wp)
    {
        Vector3 destination = wp.position;
        faceDirection(transform.position, destination);
        ChangeMovement(destination, true, velocity);
    }
}
