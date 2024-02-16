using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointDrawer : MonoBehaviour
{
    public List<Waypoint> waypoints;

    public void SetWaypoints(List<Waypoint> waypoints)
    {
        this.waypoints = waypoints;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (waypoints != null)
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                // Draw a sphere at each waypoint position
                Gizmos.DrawSphere(waypoints[i].position, 0.2f);

                // If it's not the last waypoint, draw a line to the next one
                if (i < waypoints.Count - 1)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
            }
        }
    }
}

