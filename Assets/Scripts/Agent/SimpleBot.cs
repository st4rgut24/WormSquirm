using System;
using UnityEngine;

public class SimpleBot : Bot
{

    protected override void Awake()
    {
        addNoise = false;
        botType = BotManager.BotType.Simp;
        base.Awake();
    }

    protected override bool IsReachedFinalDestination(Waypoint finalWP)
    {
        return finalWP.position == transform.position;
    }

    protected override void ReachDestination()
    {
        // Debug.Log("Reached destination");
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

