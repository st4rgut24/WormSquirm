using UnityEngine;

public class Sandbox : MonoBehaviour
{
    public Transform center; // Assign the center of the circle in the Unity Editor
    public Vector3 normalVector; // Assign the normal vector to the circle in the Unity Editor
    public float radius = 3;

    void Start()
    {
        // Calculate the radius
        // Optional: Draw the circle in the scene view
        DrawCircle(center.position, normalVector, radius, 3, Color.green);
    }

    void OnDrawGizmos()
    {
        // Visualize the normal vector
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(center.position, center.position + normalVector);

        // Visualize the center
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(center.position, 0.1f);
    }

    // Function to draw the circle in the scene view
    void DrawCircle(Vector3 center, Vector3 normal, float radius, int resolution, Color color)
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = resolution + 1;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        for (int i = 0; i <= resolution; i++)
        {
            float angle = i * (360f / resolution);
            Quaternion rotation = Quaternion.AngleAxis(angle, normal);
            Vector3 point = center + rotation * (Vector3.forward * radius);
            lineRenderer.SetPosition(i, point);
        }
    }
}