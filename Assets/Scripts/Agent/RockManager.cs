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
        TunnelActionManager.OnIntersectTunnel += OnCreateRockAtTunnelIntersection;
        TunnelActionManager.OnCreateTunnel += OnCreateRockAtTunnelCreation;
    }

    /// <summary>
    /// Create a debris field at the site of impact
    /// </summary>
    /// <param name="transform">Agent who initiated the dig</param>
    /// <param name="spawnPos">Where the debris should be created</param>
    private void CreateDebris(Transform transform, Vector3 spawnPos)
    {
        GameObject Debris = AgentManager.Instance.CreateAgent(DebrisPrefab);
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

    void OnCreateRockAtTunnelIntersection(Transform playerTransform, GameObject prevTunnel, Heading heading, bool extendsTunnel, Ring prevRing, HitInfo hitInfo)
    {
        CreateDebris(playerTransform, prevRing.center);
    }

    void OnCreateRockAtTunnelCreation(Transform playerTransform, Heading heading, Ring prevRing)
    {
        CreateDebris(playerTransform, prevRing.center);
    }

    private void OnDisable()
    {
        TunnelActionManager.OnIntersectTunnel -= OnCreateRockAtTunnelIntersection;
        TunnelActionManager.OnCreateTunnel -= OnCreateRockAtTunnelCreation;
    }
}

