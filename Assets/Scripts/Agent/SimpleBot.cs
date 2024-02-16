using System;
using UnityEngine;

public class SimpleBot : Bot
{
    protected override void Awake()
    {
        botType = BotManager.BotType.Simp;
        base.Awake();
    }

    protected override bool IsReachedFinalDestination(Waypoint finalWP)
    {
        return finalWP.position == transform.position;
    }

    protected override void ReachDestination()
    {
        notifyDig(transform.forward);
    }

    protected override void SetObjective()
    {
        GameObject simpObj = GameObject.Find("SimpObjective");
        objective = simpObj.transform;
    }
}

