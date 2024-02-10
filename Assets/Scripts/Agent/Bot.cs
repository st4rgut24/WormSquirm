using System;
using UnityEngine;

public abstract class Bot : Agent
{
    // TODO: how to set the current segment of a bot. should we do it the same way as player agent or use the route waypoints to update less frequently?
    public BotManager.BotType botType;

    public Transform objective;

    protected Vector3 startLocation = DefaultUtils.DefaultVector3;
    protected Vector3 endLocation;

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
        transform.position = startLocation;
    }

    public void setRoute(Route route)
    {
        this.route = route;

        setNextDestination();        
    }

    public void setNextDestination()
    {
        Route.MiniRoute miniRoute = route.GetNewMiniRoute();

        if (startLocation == DefaultUtils.DefaultVector3)
        {
            transform.position = miniRoute.start; // initialize bot position
        }

        startLocation = miniRoute.start;
        endLocation = miniRoute.end;
    }

    /// <summary>
    /// Face the direction of travel
    /// </summary>
    void faceDirection()
    {
        Vector3 moveDirection = (endLocation - startLocation).normalized;

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
        if (transform.position == endLocation) // reached destination
        {
            Route.MiniRoute miniRoute = route.GetMiniRoute();

            if (miniRoute.endSegment != null)
            {
                UpdateSegment(miniRoute.endSegment);
            }

            //Vector3 rayDir = miniRoute.end - miniRoute.start;
            //Debug.DrawRay(miniRoute.start, rayDir, Color.red);

            if (miniRoute.isFinalWaypoint)
            {
                Debug.Log("Reached final waypoint");
                ReachDestination();
            }
            else
            {
                setNextDestination();
            }

            // for now bots can't dig
            //notifyDig(transform.forward);
        }
        else
        {
            Move();
        }
    }

    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }

    private void Move()
    {
        faceDirection();
        ChangeMovement(endLocation, true, velocity);
    }
}
