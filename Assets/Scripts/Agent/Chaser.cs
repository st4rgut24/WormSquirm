﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : Bot
{
    protected float stoppingDistance = 2;

    public enum ChooseStrategy
    {
        Random
    }

    protected override void Awake()
    {
        botType = BotManager.BotType.Chaser;
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }


    public Transform ChooseTarget(ChooseStrategy strategy)
    {
        switch (strategy)
        {
            case ChooseStrategy.Random:
            default:
                PlayerManager pm = AgentManager.Instance.playerManager;
                List<GameObject> Players = pm.Players;

                if (Players.Count == 0)
                {
                    return null;
                }

                int randomIndex = UnityEngine.Random.Range(0, Players.Count);

                // Get the random GameObject
                GameObject randomObject = Players[randomIndex];
                return randomObject.transform;
        }
    }

    protected override void SetObjective()
    {
        objective = ChooseTarget(ChooseStrategy.Random);
    }

    protected override bool IsReachedFinalDestination(Waypoint finalWP)
    {
        float distToFinalWP = Vector3.Distance(transform.position, finalWP.position);

        return distToFinalWP <= stoppingDistance;
    }

    protected override void ReachDestination()
    {
        AbortMovement();
        SetObjective();
        BotManager.SetBotRoute(this);
    }
}

