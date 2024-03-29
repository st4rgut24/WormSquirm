﻿using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

/// <summary>
/// An agent whose movements are automated, not controlled by player
/// </summary>
public abstract class Automaton : Agent
{
    public float velocity; // Configurable velocity

    protected Route route;

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

    public virtual bool initRoute(Route route)
    {
        bool isAddFirstRoute = isFirstRoute();
        this.route = route;

        if (isAddFirstRoute)
        {
            transform.position = GetSpawnLocation();

            // initialize the direction bot is facing
            Waypoint CurWP = route.GetCurWaypoint();
            Waypoint NextWP = route.GetNextWaypoint();

            SetDirection(CurWP, NextWP);
            //this.FaceMovementDirection(CurWP.position, NextWP.position);
        
        }


        return isAddFirstRoute;
    }

    private void SetDirection(Waypoint CurWP, Waypoint NextWP)
    {
        if (CurWP != null && NextWP != null)
        {
            Vector3 initDir = (NextWP.position - CurWP.position).normalized;
            transform.rotation = Quaternion.LookRotation(initDir, Vector3.up);
        }
        else
        {
            Debug.LogWarning("Cannot set direction, missing waypoints");
        }
    }

    public bool isFirstRoute()
    {
        return this.route == null;
    }

    protected Vector3 GetMoveDirection(Vector3 destination)
    {
        Vector3 moveDir = destination - transform.position;
        return moveDir.normalized;
    }

    protected virtual void FixedUpdate()
    {
        if (route == null || hasDied)
        {
            return;
        }

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
        else if (curWP != null)
        {
            AdvanceToCurrentWP(curWP);
        }
        else
        {
            Debug.LogError("Current waypoint is undefined, and we have not reached final destination");
        }
    }

    protected void AdvanceToCurrentWP(Waypoint curWP)
    {
        if (transform.position == curWP.position) // reached destination
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

