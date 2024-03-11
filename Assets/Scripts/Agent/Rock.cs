using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

/// <summary>
/// A rolling type of agent
/// </summary>
public class Rock : Automaton
{
    private void Awake()
    {

    }

    public void SelfDestruct()
    {
        GameObject.Destroy(gameObject);
    }

    protected override void PlayFinalDestinationAnim()
    {
        // TODO: Replace with animation of rock breaking apart
        SelfDestruct();
    }

    protected override bool IsReachedFinalDestination(Waypoint finalWP)
    {
        return transform.position.Equals(finalWP.position);
    }

    protected override void ReachDestination()
    {
        AbortMovement();
    }

    protected override void Rotate(Vector3 moveDirection)
    {
        // Rotate the bot to face the direction it is moving in
        if (moveDirection != Vector3.zero)
        {
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, moveDirection);

            // Calculate the target rotation based on the rotation axis and speed
            Quaternion turnRotation = Quaternion.AngleAxis(Consts.rockRotationSpeed * Time.deltaTime, rotationAxis);
            Quaternion targetRotation = turnRotation * transform.rotation;
            Debug.Log("rotate rock " + targetRotation);
            ChangeRotation(targetRotation, Consts.rockRotationSpeed);
        }
    }

    protected override void Move(Waypoint wp)
    {
        Vector3 destination = wp.position;

        Vector3 moveDir = GetMoveDirection(destination);
        Rotate(moveDir);

        ChangeMovement(destination, true, velocity);
    }

    protected override Vector3 GetSpawnLocation()
    {
        return curSegment.GetCenterLineCenter();
    }
}

