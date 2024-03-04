using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public ToolType toolType;
    public BoxCollider weaponCollider; // used to trigger attacks

    protected float damage;

    protected Camera playerCamera;

    protected virtual void Start()
    {
        DisengageWeapon();
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
        weaponCollider.enabled = true;
        ToolManager.Instance.PlayWeaponAnim(toolType);
        // damage the gameobject that weapon collider intersects with her
    }

    public void DisengageWeapon()
    {
        weaponCollider.enabled = false;
    }

    //Upon collision with another GameObject, this GameObject will reverse direction
    private void OnTriggerEnter(Collider other)
    {
        //Transform ancestor = TransformUtils.GetAncestorMatchTag(other.transform, Consts.EnemyTag);

        if (other.transform.CompareTag(Consts.EnemyTag))
        {
            Bot bot = other.gameObject.GetComponent<Bot>();

            // Debug.Log("Deal damage to enemy " + other.gameObject.name);
            bot.TakeDamage(damage);
        }
    }
}