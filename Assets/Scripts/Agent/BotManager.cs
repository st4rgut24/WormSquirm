using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Manages the creation, destruction and decision-making behavior Bots in the game. Operates the 'swarm'
/// </summary>
public class BotManager : Singleton<BotManager>
{
    // Testing
    public Transform SimpStartBlock;
    public Transform SimpWP1;
    public Transform[] SimpWPs;

    public GameObject Chaser;
    public GameObject SimpBot;

    public const float BotHealth = 10;

    public int maxBots = 1; // the upper limit of active Bots in the game
    
    public float spawnFrequency = 15; // the interval between Bot spawns

    Dictionary<Transform, List<Bot>> ObjectiveDict; // dictionary of target transforms and their respective chasers

    RouteFactory routeMaker;

	List<Bot> bots;

    public event Action<GameObject> DestroyBotEvent;

    public enum BotType {
        Chaser,
        Simp
    }

    public int spawnDistance = 13; // number of segments away from the player the bot should spawns

    protected void OnEnable()
    {
        TunnelCreatorManager.OnAddCreatedTunnel += OnAddCreatedTunnel;
        TunnelIntersectorManager.OnAddIntersectedTunnelSuccess += OnAddIntersectedTunnel;
        Disabler.OnDisableTunnels += OnTunnelDisabled;
    }

    protected void Awake()
    {
        bots = new List<Bot>();
        ObjectiveDict = new Dictionary<Transform, List<Bot>>();

        // Testing
        SimpWPs = new Transform[] { SimpStartBlock, SimpWP1 };
    }

    // Use this for initialization
    void Start()
	{
        StartCoroutine(SpawnAtInterval());
    }

    void OnAddIntersectedTunnel(Transform playerTransform, SegmentGo segment, GameObject prevTunnel, List<GameObject> intersectedTunnels)
    {
        AddBotToSegment(playerTransform, segment);
    }

    void OnAddCreatedTunnel(Transform playerTransform, SegmentGo segment, GameObject prevTunnel)
    {
        AddBotToSegment(playerTransform, segment);
    }

    void AddBotToSegment(Transform playerTransform, SegmentGo segment)
    {
        if (playerTransform.gameObject.CompareTag(Consts.PlayerTag))
        {
            GameObject botGo = Spawn(BotType.Chaser);
            Bot bot = botGo.GetComponent<Bot>();

            bot.curSegment = SegmentManager.Instance.GetSegmentFromObject(segment.getTunnel());
            InitBot(bot);
        }
    }

    public void OnTunnelDisabled(List<GameObject> disabledTunnels)
    {
        // remove bots that are within the disabled tunnels
        bots.ForEach((bot) =>
        {
            GameObject botTunnel = bot.curSegment.tunnel;
            disabledTunnels.ForEach((tunnel) => // check if bot's tunnel is one of the disabled tunnels
            {
                if (botTunnel == tunnel)
                {
                    RemoveBot(bot);
                }
            });
        });
    }

    /// <summary>
    /// Removes all references to the bot
    /// </summary>
    /// <param name="bot">the bot to remove</param>
    public void RemoveBot(Bot bot)
    {
        bots.Remove(bot);
        Transform botObjective = bot.objective;

        if (ObjectiveDict.ContainsKey(botObjective))
        {
            ObjectiveDict[botObjective].Remove(bot);
        }
        // todo: Remove from main player's list of colliders if this bot was in contact with main player
        DestroyBotEvent?.Invoke(bot.gameObject);
        bot.Destroy();
    }

    GameObject Spawn(BotType type)
    {
        GameObject BotGo = null;

        if (type == BotType.Chaser)
        {
            BotGo = AgentManager.Instance.CreateAgent(Chaser); 
        }
        else if (type == BotType.Simp)
        {
            BotGo = AgentManager.Instance.CreateAgent(SimpBot);
        }

        return BotGo;
    }

    /// <summary>
    /// Set the bot's path and record it
    /// </summary>
    /// <param name="bot"></param>
    void InitBot(Bot bot)
    {
        SetBotRoute(bot);
        bots.Add(bot);

        if (ObjectiveDict.ContainsKey((bot.objective))) {
            ObjectiveDict[bot.objective].Add(bot);
        }
        else
        {
            ObjectiveDict[bot.objective] = new List<Bot>() { bot };
        }
    }

    IEnumerator SpawnAtInterval()
    {
        while (true)
        {
            if (bots.Count <  maxBots)
            {
                // temporary (Simp bot is just for testing)

                GameObject botGo = bots.Count == 0 ? Spawn(BotType.Simp) : Spawn(BotType.Chaser);
                Bot bot = botGo.GetComponent<Bot>();
                try
                {
                    InitBot(bot);
                }
                catch (System.Exception error)
                {
                    bots.Remove(bot);
                    Destroy(botGo);
                    // Debug.Log("Error creating bot: " + error.Message);
                    // Debug.Log(error.StackTrace);
                }
            }

            yield return new WaitForSeconds(spawnFrequency);
        }

    }

    /// <summary>
    /// Give the Bot an initial destination 
    /// </summary>
    /// <param name="bot">the bot</param>
    public static void SetBotRoute(Bot bot)
    {
        RouteStrat strat;

        if (bot.botType == BotType.Simp)
        {
            strat = RouteStrat.StraightPath;
        }
        else
        {
            strat = RouteStrat.FollowSegment;
        }

        Route route = RouteFactory.Get(strat, bot, bot.objective);

        // TESTING WAYPOINTS
        WaypointDrawer wpDrawer = GameObject.Find("WaypointDrawer").GetComponent<WaypointDrawer>();
        // Debug.Log("Set " + route.waypoints.Count + " waypoints");
        wpDrawer.SetWaypoints(route.waypoints);

        bot.initRoute(route);
    }

    // Update is called once per frame
    void Update()
	{
			
	}

    protected void OnDisable()
    {
        Disabler.OnDisableTunnels -= OnTunnelDisabled;
        TunnelCreatorManager.OnAddCreatedTunnel -= OnAddCreatedTunnel;
        TunnelIntersectorManager.OnAddIntersectedTunnelSuccess -= OnAddIntersectedTunnel;
    }
}

