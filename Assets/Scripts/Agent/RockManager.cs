using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RockManager : Singleton<RockManager>
{

    [SerializeField]
    private GameObject RockPrefab;

    [SerializeField]
    private GameObject DebrisPrefab;

    public static float RockHealth = 1000f;

    // TODO: Rock Manager determines the HP of the rocks. 'Tougher' rocks can crush a player
    private void OnEnable()
    {
        Agent.OnDig += CreateDebris;
    }

    /// <summary>
    /// Create a debris field at the site of impact
    /// </summary>
    /// <param name="transform">Agent who initiated the dig</param>
    /// <param name="digDirection">Where the debris should be created</param>
    private void CreateDebris(Transform transform, Vector3 digDirection)
    {
        GameObject Debris = AgentManager.Instance.CreateAgent(DebrisPrefab);
        Vector3 spawnPos = digDirection.normalized + transform.position;
        Debris.transform.position = spawnPos;

        Rock debrisAgent = Debris.GetComponent<Rock>();

        StartCoroutine(debrisAgent.DieCoroutine());
    }


    public GameObject Spawn(Segment segment)
    {        
        GameObject RockGo = AgentManager.Instance.CreateAgent(RockPrefab);

        Rock rockAgent = RockGo.GetComponent<Rock>();
        rockAgent.curSegment = segment;
        Route route = RouteFactory.Get(RouteStrat.Gravity, rockAgent, null);

        WaypointDrawer wpDrawer = GameObject.Find(Consts.RockRouteDrawer).GetComponent<WaypointDrawer>();
        wpDrawer.SetWaypoints(route.waypoints, Color.blue);

        // assign route to RockAgent
        rockAgent.initRoute(route);

        return RockGo;
    }

    private void OnDisable()
    {
        Agent.OnDig -= CreateDebris;
    }
}

