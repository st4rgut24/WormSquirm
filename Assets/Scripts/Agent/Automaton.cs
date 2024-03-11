using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

/// <summary>
/// An agent whose movements are automated, not controlled by player
/// </summary>
public abstract class Automaton : Agent
{
    public float velocity; // Configurable velocity

    protected Route route;

    protected bool hasRoute = false;

    /// <summary>
    /// Play this animation when final destination is reached
    /// </summary>
    protected abstract void PlayFinalDestinationAnim();

    protected abstract bool IsReachedFinalDestination(Waypoint finalWP);

    protected abstract void ReachDestination();

    protected abstract void Move(Waypoint wp);

    protected abstract void Rotate(Vector3 direction);

    protected abstract Vector3 GetSpawnLocation();

    protected virtual void ReachWaypoint()
    {
        route.AdvanceWaypoint();
    }

    public void initRoute(Route route)
    {
        this.route = route;

        if (!hasRoute)
        {
            hasRoute = true;
            transform.position = GetSpawnLocation();

            // initialize the direction bot is facing
            Waypoint CurWP = route.GetCurWaypoint();
            Waypoint NextWP = route.GetNextWaypoint();
            //this.FaceMovementDirection(CurWP.position, NextWP.position);
            Vector3 initDir = (NextWP.position - CurWP.position).normalized;
            transform.rotation = Quaternion.LookRotation(initDir, Vector3.up);
        }
    }

    protected Vector3 GetMoveDirection(Vector3 destination)
    {
        Vector3 moveDir = destination - transform.position;
        return moveDir.normalized;
    }

    protected virtual void FixedUpdate()
    {
        Waypoint curWP = route.GetCurWaypoint();
        Waypoint finalWP = route.GetLastWaypoint();

        //// Debug.Log("Waypoint is " + curWP.position);

        if (IsReachedFinalDestination(finalWP))
        {
            PlayFinalDestinationAnim();
            //charAnimator.TriggerAnimation(isAttackingAnimName);

            //// Debug.Log("Reached final waypoint");
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
}

