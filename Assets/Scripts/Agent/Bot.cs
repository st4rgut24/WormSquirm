using System;
using UnityEngine;

public abstract class Bot : Agent
{
    // TODO: how to set the current segment of a bot. should we do it the same way as player agent or use the route waypoints to update less frequently?
    public BotManager.BotType botType;

    public Transform objective;

    public float velocity = .00001f; // Configurable velocity

    protected float startTime;

    protected bool reachedDestination;

    protected Route route;

    protected abstract void SetObjective();

    protected abstract void ReachDestination();

    protected virtual void Awake()
    {
        reachedDestination = true;
        SetObjective();
    }

    public void setRoute(Route route)
    {
        this.route = route;
    }

    /// <summary>
    /// Face the direction of travel
    /// </summary>
    void faceDirection(Vector3 dest)
    {
        Vector3 moveDirection = (dest - transform.position).normalized;

        // Rotate the bot to face the direction it is moving in
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        }
    }

    protected override void Start()
    {
        base.Start();
        startTime = Time.time;

        Segment initSegment = this.route.GetInitSegment();

        if (initSegment != null)
        {
            AgentManager.Instance.InitTransformSegmentDict(transform, initSegment);
        }
    }

    private void FixedUpdate()
    {
        Waypoint destWP = route.GetCurWaypoint();

        if (transform.position == destWP.position) // reached destination
        {
            if (destWP.segment != null)
            {
                UpdateSegment(destWP.segment);
            }

            //Vector3 rayDir = miniRoute.end - miniRoute.start;
            //Debug.DrawRay(miniRoute.start, rayDir, Color.red);

            if (route.IsFinalWaypoint(destWP))
            {
                Debug.Log("Reached final waypoint");
                ReachDestination();
            }
            else
            {
                route.AdvanceWaypoint();
            }

            // for now bots can't dig
            //notifyDig(transform.forward);
        }
        else
        {
            Move(destWP);
        }
    }

    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }

    private void Move(Waypoint wp)
    {
        Vector3 destination = wp.position;
        faceDirection(destination);
        ChangeMovement(destination, true, velocity);
        Debug.Log("Move to position " + transform.position);
    }
}
