using UnityEngine;

public abstract class Bot : Agent
{
    public Transform objective;

    protected Vector3 startLocation = DefaultUtils.DefaultVector3;
    protected Vector3 endLocation;

    public float velocity = .5f; // Configurable velocity

    protected float startTime;

    protected bool reachedDestination;

    protected Route route;

    protected abstract void SetObjective();

    private void Awake()
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
    }

    private void FixedUpdate()
    {
        if (transform.position == endLocation) // reached destination
        {
            Route.MiniRoute miniRoute = route.GetMiniRoute();
            if (miniRoute.isFinalWaypoint)
            {
                Debug.Log("Reached final waypoint");
                // emit event signalling Bot has reached destination, so it can be assigned a new destination
            }
            else
            {
                setNextDestination();
            }

            notifyDig(transform.forward);
        }
        else
        {
            Move();
        }
    }

    private void Move()
    {
        faceDirection();
        ChangeMovement(endLocation, true, velocity);
    }
}
