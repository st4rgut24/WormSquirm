using UnityEngine;
using System.Collections;

public class GateKeeper : Bot
{
    Gate gate;

    protected override bool IsReachedFinalDestination(Waypoint finalWP)
    {
        return finalWP.position.Equals(route.GetCurrentPosition());
    }

    /// <summary>
    /// left empty on purpose because there is nothing to do once a gatekeeper has arrived
    /// </summary>
    protected override void ReachDestination()
    {
    }

    protected override void PlayFinalDestinationAnim()
    {
        charAnimator.Idle();
    }

    public override Vector3 GetDestination()
    {
        Vector3 gatePos = objective.position;
        Vector3 gateForward = gate.transform.forward;

        Vector3 keeperPos = gatePos - gateForward;

        return keeperPos;
    }

    protected override void SetObjective()
    {
        gate = GateManager.Instance.GetNewestGate();
        objective = gate.transform;
    }
}

