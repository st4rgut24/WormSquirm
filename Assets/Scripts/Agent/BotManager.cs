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
    public Transform TunnelMakerStartBlock;
    public Transform TunnelMakerWP1;
    public Transform[] TunnelMakerWPs;

    public GameObject Chaser;
    public GameObject GateKeeper;
    public GameObject TunnelMaker;

    public const float BotHealth = 10;

    public int maxBots = 1; // the upper limit of active Bots in the game
    
    public float spawnFrequency = 15; // the interval between Bot spawns

    Dictionary<Transform, List<Bot>> ObjectiveDict; // dictionary of target transforms and their respective chasers

    RouteFactory routeMaker;

	List<Bot> bots;

    public event Action<GameObject> DestroyBotEvent;

    public enum BotType {
        Chaser,
        InitTunnelMaker,
        GateKeeper
    }

    public int spawnDistance = 13; // number of segments away from the player the bot should spawns

    protected void OnEnable()
    {
        Disabler.OnDisableTunnels += OnTunnelDisabled;
        GateManager.CreateGateEvent += OnAddGate;
    }

    protected void Awake()
    {
        bots = new List<Bot>();
        ObjectiveDict = new Dictionary<Transform, List<Bot>>();

        // Testing intersection with an existing tunnel
        TunnelMakerWPs = new Transform[] { TunnelMakerStartBlock, TunnelMakerWP1 };
    }

    // Use this for initializing tunnel
    void Start()
    {
        GameObject botGo = Spawn(BotType.InitTunnelMaker);
        Bot bot = botGo.GetComponent<Bot>();

        InitBot(bot);
    }

    public void AddBotToSegment(Transform playerTransform, Segment segment)
    {
        if (playerTransform.gameObject.CompareTag(Consts.MainPlayerTag))
        {
            GameObject botGo = Spawn(BotType.Chaser);
            Bot bot = botGo.GetComponent<Bot>();

            bot.curSegment = segment;
            InitBot(bot);
        }
    }

    /// <summary>
    /// Adds a bot to control the gate
    /// </summary>
    /// <param name="gate">The gate to control</param>
    public void OnAddGate()
    {
        GameObject GateKeeperGo = Spawn(BotType.GateKeeper);
        GateKeeper GateKeeperBot = GateKeeperGo.GetComponent<GateKeeper>();

        InitBot(GateKeeperBot);
    }

    public void OnTunnelDisabled(List<GameObject> disabledTunnels)
    {
        // remove bots that are within the disabled tunnels
        bots.ForEach((bot) =>
        {
            GameObject botTunnel = bot.curSegment?.tunnel;
            disabledTunnels.ForEach((tunnel) => // check if bot's tunnel is one of the disabled tunnels
            {
                if (botTunnel != null && botTunnel == tunnel)
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
        else if (type == BotType.GateKeeper)
        {
            BotGo = AgentManager.Instance.CreateAgent(GateKeeper);
        }
        else if (type == BotType.InitTunnelMaker)
        {
            BotGo = AgentManager.Instance.CreateAgent(TunnelMaker);
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

    /// <summary>
    /// Give the Bot an initial destination 
    /// </summary>
    /// <param name="bot">the bot</param>
    public static void SetBotRoute(Bot bot)
    {
        RouteStrat strat;

        if (bot.botType == BotType.InitTunnelMaker)
        {
            strat = RouteStrat.InitPath;
        }
        else if (bot.botType == BotType.GateKeeper)
        {
            strat = RouteStrat.Stationary;
        }
        else
        {
            strat = RouteStrat.FollowSegment;
        }

        Vector3 destination = bot.GetDestination();
        Route route = RouteFactory.Get(strat, bot, bot.objective, destination, bot.addNoise); // want bots to follow slightly different routes, so add noise

        // TESTING WAYPOINTS
        WaypointDrawer wpDrawer = GameObject.Find(Consts.BotRouteDrawer).GetComponent<WaypointDrawer>();
        // Debug.Log("Set " + route.waypoints.Count + " waypoints");
        wpDrawer.SetWaypoints(route.waypoints, Color.red);

        bot.initRoute(route);
    }

    protected void OnDisable()
    {
        Disabler.OnDisableTunnels -= OnTunnelDisabled;
        GateManager.CreateGateEvent -= OnAddGate;
    }
}

