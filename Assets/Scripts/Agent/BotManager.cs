using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// Manages the creation, destruction and decision-making behavior Bots in the game. Operates the 'swarm'
/// </summary>
public class BotManager : AgentManager
{
    public GameObject Chaser;
    public GameObject SimpBot;

    public int maxBots = 1; // the upper limit of active Bots in the game
    
    public float spawnFrequency = 1; // the interval between Bot spawns

    public Transform SimpStartBlock;

    Dictionary<Transform, List<Bot>> ChasingDict; // dictionary of target transforms and their respective chasers

    RouteFactory routeMaker;

	List<Bot> bots;

    public enum BotType {
        Chaser,
        Simp
    }

    public int spawnDistance = 13; // number of segments away from the player the bot should spawns

    protected override void Awake()
    {
        base.Awake();
        bots = new List<Bot>();
        ChasingDict = new Dictionary<Transform, List<Bot>>(); 
    }

    // Use this for initialization
    void Start()
	{
        StartCoroutine(SpawnAtInterval());
    }



    GameObject Spawn(BotType type)
    {
        GameObject Bot = null;

        if (type == BotType.Chaser)
        {
            Bot = CreateAgent(Chaser); 
        }
        else if (type == BotType.Simp)
        {
            Bot = CreateAgent(SimpBot);
        }

        return Bot;
    }

    IEnumerator SpawnAtInterval()
    {
        while (true)
        {
            if (bots.Count <  maxBots)
            {
                //GameObject botGo = Spawn(BotType.Chaser);
                GameObject botGo = Spawn(BotType.Simp);
                Bot bot = botGo.GetComponent<Bot>();

                try
                {
                    //Route route = GetBotRoute(bot.objective, RouteStrat.FollowSegment);
                    Route route = GetBotRoute(bot.objective, RouteStrat.StraightPath);
                    bot.setRoute(route);
                    bots.Add(bot);
                }
                catch (System.Exception error)
                {
                    bots.Remove(bot);
                    Destroy(botGo);
                    Debug.Log("Error creating bot: " + error.Message);
                    Debug.Log(error.StackTrace);
                }
            }

            yield return new WaitForSeconds(spawnFrequency);
        }

    }

    /// <summary>
    /// Define behavior when the bot has reached its route
    /// </summary>
    /// <param name="bot">the bot</param>
    public void OnReachedRoute(Bot bot)
    {
        Debug.Log("Reached Route!!! Now what should I do?");
    }

    /// <summary>
    /// Give the Bot an initial destination 
    /// </summary>
    /// <param name="bot">the bot</param>
    Route GetBotRoute(Transform targetTransform, RouteStrat strat)
    {
        return RouteFactory.Get(strat, targetTransform);
    }

    // Update is called once per frame
    void Update()
	{
			
	}
}

