using UnityEngine;

public abstract class Bot : Agent
{
    public Transform objective;

    protected Vector3 startLocation;
    protected Vector3 endLocation;

    public float velocity = 5f; // Configurable velocity

    protected float startTime;

    protected bool reachedDestination;

    protected Route route;

    protected abstract void SetObjective();

    private void Awake()
    {
        reachedDestination = true;
        SetObjective();
    }

    public void setRoute(Route route)
    {
        this.route = route;

        setNextDestination();        
    }

    public void setNextDestination()
    {
        Route.MiniRoute miniRoute = route.GetNewMiniRoute();

        startLocation = miniRoute.start;
        endLocation = miniRoute.end;
        this.reachedDestination = false;
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
        if (!reachedDestination)
        {
            if (route == null)
            {
                throw new System.Exception("Route is not available for bot");
            }

            Move(); 
        }
        else
        {
            Route.MiniRoute miniRoute = route.GetMiniRoute();
            if (miniRoute.isFinalWaypoint)
            {
                // emit event signalling Bot has reached destination, so it can be assigned a new destination
            }
            else
            {
                setNextDestination();
            }

            notifyDig(transform.forward);
        }
    }

    private void Move()
    {
        faceDirection();
        // Calculate the current progress based on time and velocity
        float journeyLength = Vector3.Distance(startLocation, endLocation);
        float journeyTime = journeyLength / velocity;
        float fractionOfJourney = (Time.time - startTime) / journeyTime;

        // Move the bot towards the destination
        transform.position = Vector3.Lerp(startLocation, endLocation, fractionOfJourney);

        // If the bot reaches the destination, reset the start time for future movements
        if (fractionOfJourney >= 1.0f)
        {
            startTime = Time.time;
            reachedDestination = true;
        }
    }
}
