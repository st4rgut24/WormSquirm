using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointDrawer : MonoBehaviour
{
    public List<Waypoint> waypoints;
    Color gizmoColor;

    public void SetWaypoints(List<Waypoint> waypoints, Color color)
    {
        this.waypoints = waypoints;
        this.gizmoColor = color;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

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

