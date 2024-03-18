using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class AgentManager : Singleton<AgentManager>
{
    public bool SpawnRock;
    public bool SpawnBot;

    public Dictionary<Transform, Segment> TransformSegmentDict; // <GameObject Transform, Last Enclosing Segment>

    public Transform agentsParent;

    public static event Action<Transform> OnSpawn;

    private void OnEnable()
    {
        SegmentManager.EnterNewSegmentEvent += UpdateSegment;

        TunnelCreatorManager.OnAddCreatedTunnel += OnAddCreatedTunnel;
        TunnelIntersectorManager.OnAddIntersectedTunnelSuccess += OnAddIntersectedTunnel;
    }

    private void Awake()
    {
        TransformSegmentDict = new Dictionary<Transform, Segment>();
    }

    void Spawn(Transform playerTransform, SegmentGo segmentGo, GameObject prevTunnel)
    {
        Segment segment = SegmentManager.Instance.GetSegmentFromSegmentGo(segmentGo);
        // TODO: Game spawning logic to decide what to spawn
        if (SpawnRock)
        {
            if (SegmentUtils.IsSegmentsDownhill(segment, prevTunnel))
            {
                RockManager.Instance.Spawn(segment);
            }
            else
            {
                Debug.LogWarning("Segments do not allow rock to roll downhill");
            }
        }
        else if (SpawnBot)
        {
            BotManager.Instance.AddBotToSegment(playerTransform, segment);
        }
    }

    void OnAddIntersectedTunnel(Transform playerTransform, SegmentGo segment, GameObject prevTunnel, List<GameObject> startIntersectedTunnels, List<GameObject> endIntersectedTunnels)
    {
        Spawn(playerTransform, segment, prevTunnel);
    }

    void OnAddCreatedTunnel(Transform playerTransform, SegmentGo segment, GameObject prevTunnel)
    {
        Spawn(playerTransform, segment, prevTunnel);
    }

    /// <summary>
    /// The segment 2 transform dictionary should be initialized when a tunnel is first created
    /// </summary>
    /// <param name="transform">player transform</param>
    /// <param name="segment">segment tunnel</param>
    public void InitSegment(Transform transform, Segment segment)
    {
        Agent agent = transform.gameObject.GetComponent<Agent>();
        agent.InitSegment(segment);
    }

    public void SetTransformSegmentDict(Transform transform, Segment segment)
    {
        TransformSegmentDict[transform] = segment;
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
    public void UpdateSegment(Transform transform, Segment segment)
    {
        Agent movedAgent = transform.gameObject.GetComponent<Agent>();
        movedAgent.UpdateSegment(segment);
        float xRot = DirectionUtils.GetUpDownRotation(transform.forward, movedAgent.curSegmentForward);
        //Vector3 rotation = new Vector3(xRot, transform.eulerAngles.y, transform.eulerAngles.z);
        movedAgent.ChangeVerticalRotation(xRot, Consts.defaultRotationSpeed); // instantaneous update
        //// Debug.Log("Enter new segment " + segment.tunnel.name + ". Target rotation is " + rotation);
        TransformSegmentDict[transform] = segment;
    }

    public GameObject CreateAgent(GameObject agentGo)
    {
        GameObject agent = Instantiate(agentGo, agentsParent);
        OnSpawn?.Invoke(agent.transform);

        return agent;
    }

    private void OnDisable()
    {
        SegmentManager.EnterNewSegmentEvent -= UpdateSegment;

        TunnelCreatorManager.OnAddCreatedTunnel -= OnAddCreatedTunnel;
        TunnelIntersectorManager.OnAddIntersectedTunnelSuccess -= OnAddIntersectedTunnel;
    }
}

