using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    enum GameState {
        Start,
        ToTreasure,
        FromTreasure,
        Finish
    }

    GridManager gridManager;

    TunnelManager tunnelManager;

    public int agentOffset = 12; // how many units away from end of a newly created tunnel a player is
    public int initAgentOffset = 4; // how many units away fro the beginning of a newly created tunnel a player is spawned at start of game

    public Vector3Int origin = new Vector3Int(0, 0, 0);
    public Vector3Int gameStartLocation = new Vector3Int(50, 50, 0);

    public int xAxisLength = 100;
    public int yAxisLength = 100;
    public int zAxisLength = 100;
    // World will be a cube starting from the origin and corners along the positive axes

    GameState gameState;

    private void OnEnable()
    {
        
    }

    private void Awake()
    {
        tunnelManager = TunnelManager.Instance;
        gridManager = new GridManager();

        gameState = GameState.Start;
    }

    public void InitGame(List<Waypoint> waypoints)
    {
        if (gameState == GameState.Start && waypoints.Count >= Consts.MinSetupWPs)
        {
            int spawnIdx = waypoints.Count - 2;
            Waypoint spawnWaypoint = waypoints[spawnIdx]; // spawn at the second to last waypoint, last waypoint is tunnel end, first waypoint is start

            Vector3 playerLocation = spawnWaypoint.position + spawnWaypoint.segment.forward * initAgentOffset;
            PlayerManager.Instance.InitPlayer(playerLocation, spawnWaypoint.segment);

            // initialize the gate
            Waypoint gateWaypoint = waypoints[spawnIdx];
            GateManager.Instance.Create(gateWaypoint.segment, GateType.Key);
        }
        else
        {
            throw new System.Exception("Not the correct state for game initialization");
        }
    }

    public bool isValidPos(Vector3 pos)
    {
        Vector3 distAlongAxes = pos - GameManager.Instance.origin;

        return !(distAlongAxes.x < 0 || distAlongAxes.x > xAxisLength || distAlongAxes.y < 0 || distAlongAxes.y > yAxisLength || distAlongAxes.z < 0 || distAlongAxes.z > zAxisLength);
    }

    public Grid GetGrid(GridType type)
    {
        return gridManager.GetGrid(type);
    }
}
