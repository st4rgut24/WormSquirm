using System;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : Bot
{
    public const float damageAmt = 10f;

    protected float stoppingDistance = 1.75f;

    public enum ChooseStrategy
    {
        Random
    }

    private void OnEnable()
    {
        SegmentManager.OnEnterNewSegment += OnEnterNewSegment;
    }

    protected override void Awake()
    {
        addNoise = true;
        botType = BotManager.BotType.Chaser;

        damage = damageAmt; // amount of damage this type of Bot can inflict
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }


    /// <summary>
    /// If the chased agent has moved to another segment, reroute
    /// </summary>
    /// <param name="transform">Agent entering new segment</param>
    /// <param name="segment">entered segment</param>
    public void OnEnterNewSegment(Transform transform, Segment segment)
    {
        if (transform.gameObject == objective.gameObject)
        {
            ReachDestination();
        }
    }

    public Transform ChooseTarget(ChooseStrategy strategy)
    {
        switch (strategy)
        {
            case ChooseStrategy.Random:
            default:
                List<GameObject> Players = PlayerManager.Instance.Players;

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

    private void OnDisable()
    {
        SegmentManager.OnEnterNewSegment -= OnEnterNewSegment;
    }
}

