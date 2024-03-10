using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RockManager : Singleton<RockManager>
{
    [SerializeField]
    private GameObject RockPrefab;

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
}

