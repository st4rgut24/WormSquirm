using UnityEngine;

public class Bot : Agent
{
    public Transform goal;
    public Transform start;

    private Vector3 startLocation;
    private Vector3 endLocation;

    public float velocity = 5f; // Configurable velocity

    private float startTime;

    private void Awake()
    {
        startLocation = start.position;
        endLocation = goal.position;

        transform.position = startLocation;

        initFaceDirection();
    }

    void initFaceDirection()
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

    private void Update()
    {
        MoveBot();
    }

    private void MoveBot()
    {
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
        }
    }
}
