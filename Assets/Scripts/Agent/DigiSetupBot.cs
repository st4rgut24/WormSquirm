using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Creates the tunnels on game startup
/// </summary>
public class DigiSetupBot : DigiBot
{
    private bool isSetupCompleted = false; // this flag ensures game is setup once

    protected override void ReachDestination()
    {
        if (!isSetupCompleted)
        {
            base.ReachDestination();
                
            GameManager.Instance.InitGame(route.waypoints);

            isSetupCompleted = true;
            // Debug.Log("Reached destination");
            //notifyDig(transform.forward);
        }
    }
}

