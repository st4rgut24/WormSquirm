using UnityEngine;
using System.Collections;
using System;

public class AgentManager : Singleton<AgentManager>
{
    public PlayerManager playerManager;
    public BotManager botManager;

    public static event Action<Transform> OnSpawn;

    protected virtual void Awake()
    {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        botManager = GameObject.Find("BotManager").GetComponent<BotManager>();
    }

    protected GameObject CreateAgent(GameObject agentGo)
    {
        GameObject agent = Instantiate(agentGo);
        OnSpawn?.Invoke(agent.transform);

        return agent;
    }
}

