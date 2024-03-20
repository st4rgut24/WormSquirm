using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Creates the tunnels on game startup
/// </summary>
public class DigiSetupBot : DigiBot
{
    protected override void Awake()
    {
        base.Awake();

    }

    protected override void ReachDestination()
    {
        base.ReachDestination();
        GameManager.Instance.InitGame(route.waypoints);
        BotManager.Instance.RemoveBot(this);
    }
}

