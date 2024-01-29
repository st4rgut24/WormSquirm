using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
