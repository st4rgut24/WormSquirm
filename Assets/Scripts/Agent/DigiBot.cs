using System;
using UnityEngine;

/// <summary>
/// A bot whose sole purpose is creating tunnels
/// </summary>
public class DigiBot : Bot
{
    protected override void Awake()
    {
        addNoise = false;
        botType = BotManager.BotType.InitTunnelMaker;
        base.Awake();
    }

    protected override void PlayFinalDestinationAnim()
    {
        charAnimator.Idle();
    }

    /// <summary>
    /// Get the segment of the newly reached waypoint if it is not yet set.
    /// </summary>
    private void UpdateWaypoint(Segment segment)
    {
        Waypoint waypoint = route.GetCurWaypoint();

        if (waypoint.segment == null)
        {
            waypoint.segment = segment;
        }
    }

    public override void InitSegment(Segment segment)
    {
        base.InitSegment(segment);

        AgentManager.Instance.SetTransformSegmentDict(transform, segment);
        UpdateWaypoint(segment);
    }

    protected override bool IsReachedFinalDestination(Waypoint finalWP)
    {
        return finalWP.position == transform.position;
    }

    protected override void ReachDestination()
    {
        Debug.Log("Reached destination");
        //notifyDig(transform.forward);
    }

    protected override void ReachWaypoint()
    {        
        base.ReachWaypoint();

        notifyDig(transform.forward);
    }

    protected override void SetObjective()
    {
        GameObject simpObj = GameObject.Find("SimpObjective");
        objective = simpObj.transform;
    }
}

