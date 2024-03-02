using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// Manages the creation, destruction and decision-making behavior Bots in the game. Operates the 'swarm'
/// </summary>
public class BotManager : AgentManager
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

    Dictionary<Transform, List<Bot>> ChasingDict; // dictionary of target transforms and their respective chasers

    RouteFactory routeMaker;

	List<Bot> bots;

    public enum BotType {
        Chaser,
        Simp
    }

    public int spawnDistance = 13; // number of segments away from the player the bot should spawns

    private void OnEnable()
    {
        Disabler.OnDisableTunnels += OnTunnelDisabled;
    }

    protected override void Awake()
    {
        base.Awake();
        bots = new List<Bot>();
        ChasingDict = new Dictionary<Transform, List<Bot>>();

        // Testing
        SimpWPs = new Transform[] { SimpStartBlock, SimpWP1 };
    }

    // Use this for initialization
    void Start()
	{
        StartCoroutine(SpawnAtInterval());
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

        if (ChasingDict.ContainsKey(botObjective))
        {
            ChasingDict[botObjective].Remove(bot);
        }

        bot.Destroy();
    }

    /// <summary>
    /// For testing chasing
    /// </summary>
    public void SpawnChaser()
    {
        GameObject Bot = CreateAgent(Chaser);
        Bot bot = Bot.GetComponent<Bot>();
        SetBotRoute(bot);
        bots.Add(bot);
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
                BotType type = bots.Count == 0 ? BotType.Simp : BotType.Chaser;
                Debug.Log("Spawn bot of type " + type);
                GameObject botGo = Spawn(type);
                Bot bot = botGo.GetComponent<Bot>();

                try
                {
                    //Route route = GetBotRoute(bot.objective, RouteStrat.FollowSegment);
                    SetBotRoute(bot);
                    //Route route = GetBotRoute(bot.objective, RouteStrat.StraightPath);
                    //bot.setRoute(route);
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
        else // (bot.botType == BotType.Chaser)
        {
            strat = RouteStrat.FollowSegment;
        }

        Route route = RouteFactory.Get(strat, bot, bot.objective);

        // TESTING WAYPOINTS
        WaypointDrawer wpDrawer = GameObject.Find("WaypointDrawer").GetComponent<WaypointDrawer>();
        Debug.Log("Set " + route.waypoints.Count + " waypoints");
        wpDrawer.SetWaypoints(route.waypoints);

        bot.initRoute(route);
    }

    // Update is called once per frame
    void Update()
	{
			
	}

    private void OnDisable()
    {
        Disabler.OnDisableTunnels -= OnTunnelDisabled;
    }
}

