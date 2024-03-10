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

    protected override void Move(Waypoint wp)
    {
        Vector3 destination = wp.position;

        ChangeMovement(destination, true, velocity);
    }

    protected override Vector3 GetSpawnLocation()
    {
        return curSegment.GetCenterLineCenter();
    }
}

