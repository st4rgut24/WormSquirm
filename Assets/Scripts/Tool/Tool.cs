using System;
using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    protected Camera playerCamera;

    protected virtual void Start()
    {
        playerCamera = ToolManager.Instance.playerCamera;
    }

    public Vector3 ProjectScreenPosition(Vector2 screenPos)
    {
        // Use the screen position to create a ray from the player's perspective
        Ray ray = playerCamera.ScreenPointToRay(screenPos);

        // Debug draw the ray (optional)
        //Debug.DrawRay(ray.origin, ray.direction * 10f, Color.green);

        // Now you can use 'ray.direction' as the direction vector from the player
        // Example: You can use this direction to perform raycasting or other calculations
        return ray.direction;
    }

    /// <summary>
    /// Get the direction which the tool is being used
    /// </summary>
    public abstract Vector3 GetDirection();

    /// <summary>
    /// Apply effects of using the tool
    /// </summary>
    /// <param name="direction">direction of tool</param>
    public abstract void Use();
}