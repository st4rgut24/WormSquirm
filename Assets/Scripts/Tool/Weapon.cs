using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public ToolType toolType;
    Collider weaponCollider; // used to trigger attacks

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
    /// Apply effects of using the tool, if the weapon collider intersects with anything 
    /// </summary>
    /// <param name="direction">direction of tool</param>
    public virtual void Use()
    {
        ToolManager.Instance.PlayWeaponAnim(toolType);
        // damage the gameobject that weapon collider intersects with her
    }
}