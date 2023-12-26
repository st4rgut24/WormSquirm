using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Vector3Int origin = new Vector3Int(0, 0, 0);

    public int xAxisLength = 100;
    public int yAxisLength = 100;
    public int zAxisLength = 100;
    // World will be a cube starting from the origin and corners along the positive axes

    public bool isValidPos(Vector3 pos)
    {
        Vector3 distAlongAxes = pos - GameManager.Instance.origin;

        return !(distAlongAxes.x < 0 || distAlongAxes.x > xAxisLength || distAlongAxes.y < 0 || distAlongAxes.y > yAxisLength || distAlongAxes.z < 0 || distAlongAxes.z > zAxisLength);
    }
}
