using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    GridManager gridManager;

    TunnelManager tunnelManager;

    public int maxSegmentLength = 3; // the maximum length of a tunnel segment

    public int agentOffset = 6; // how many units away from end of a newly created tunnel a player is

    public Vector3Int origin = new Vector3Int(0, 0, 0);

    public int xAxisLength = 100;
    public int yAxisLength = 100;
    public int zAxisLength = 100;
    // World will be a cube starting from the origin and corners along the positive axes

    private void Awake()
    {
        tunnelManager = TunnelManager.Instance;
        gridManager = new GridManager();
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
