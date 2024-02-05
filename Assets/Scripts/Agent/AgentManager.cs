﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class AgentManager : Singleton<AgentManager>
{
    public Dictionary<Transform, Segment> TransformSegmentDict; // <GameObject Transform, Last Enclosing Segment>

    public PlayerManager playerManager;
    public BotManager botManager;

    public static event Action<Transform> OnSpawn;

    private void OnEnable()
    {
        SegmentManager.OnEnterNewSegment += OnEnterNewSegment;
    }

    protected virtual void Awake()
    {
        TransformSegmentDict = new Dictionary<Transform, Segment>();
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        botManager = GameObject.Find("BotManager").GetComponent<BotManager>();
    }

    /// <summary>
    /// The segment 2 transform dictionary should be initialized when a tunnel is first created
    /// </summary>
    /// <param name="transform">player transform</param>
    /// <param name="segment">segment tunnel</param>
    public void InitTransformSegmentDict(Transform transform, Segment segment)
    {
        if (!TransformSegmentDict.ContainsKey(transform))
        {
            Agent agent = transform.gameObject.GetComponent<Agent>();

            agent.curSegment = segment; 
            TransformSegmentDict[transform] = segment;
        }
    }

    public Segment GetSegment(Transform transform)
    {
        if (TransformSegmentDict.ContainsKey(transform))
        {
            return TransformSegmentDict[transform];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// When player enters new tunnel, adjust the player's angle of attack
    /// </summary>
    /// <param name="transform">player transform</param>
    /// <param name="segment">segment of tunnel player is in</param>
    public void OnEnterNewSegment(Transform transform, Segment segment)
    {
        Agent movedAgent = transform.gameObject.GetComponent<Agent>();
        movedAgent.UpdateSegment(segment);

        //if (IsControlledMovement(transform, segment))
        //{
        //   Vector3 controlledMoveDest = segment.GetClosestPointOnSegmentEdge(transform.position);
        //    movedAgent.ChangeMovement(controlledMoveDest, true, 1);
        //}

        float xRot = DirectionUtils.GetUpDownRotation(transform.forward, movedAgent.curSegmentForward);
        Vector3 rotation = new Vector3(xRot, transform.eulerAngles.y, transform.eulerAngles.z);
        movedAgent.ChangeRotation(rotation, true);
        Debug.Log("Enter new segment " + segment.tunnel.name + ". Target rotation is " + rotation);
        TransformSegmentDict[transform] = segment;
    }

    //public bool IsControlledMovement(Transform transform, Segment segment)
    //{
    //    if (TransformSegmentDict.ContainsKey(transform))
    //    {
    //        Segment prevSegment = TransformSegmentDict[transform];

    //        return TunnelManager.Instance.IsIntersectingTunnel(segment.tunnel, prevSegment.tunnel);
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    protected GameObject CreateAgent(GameObject agentGo)
    {
        GameObject agent = Instantiate(agentGo);
        OnSpawn?.Invoke(agent.transform);

        return agent;
    }

    private void OnDisable()
    {
        SegmentManager.OnEnterNewSegment -= OnEnterNewSegment;
    }
}

