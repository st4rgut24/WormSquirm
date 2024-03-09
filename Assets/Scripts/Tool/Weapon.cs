using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public bool isEquipped = false;
    public ToolType toolType;
    public BoxCollider weaponCollider; // used to trigger attacks

    protected float damage;

    protected Camera playerCamera;

    protected virtual void Start()
    {
        DisengageWeapon();
        playerCamera = ToolManager.Instance.playerCamera;
    }

    protected bool IsTouchInsideWeaponCanvas(Vector3 screenPos)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(ToolManager.Instance.WeaponCanvas, screenPos);
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
    /// Get the status of the weapon animation that the player uses when equipping the weapon
    /// </summary>
    /// <returns>returns true if the weapon's animation is paused, for example for sniping</returns>
    public abstract bool GetPauseAnimStatus();

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
        // damage the gameobject that weapon collider intersects with her
    }

    public void DisengageWeapon()
    {
        weaponCollider.enabled = false;
    }

    protected void DamageCollidedObject(Collider other)
    {
        if (other.transform.CompareTag(Consts.EnemyTag))
        {
            Bot bot = other.gameObject.GetComponent<Bot>();

            Debug.Log("Deal damage to enemy " + other.gameObject.name);
            bot.TakeDamage(damage);
        }
    }

    protected void StopWeaponAnim(bool isPaused)
    {
        ToolManager.Instance.StopWeaponAnim(toolType, isPaused);
    }

    protected void PlayWeaponAnim(bool isPaused)
    {
        ToolManager.Instance.PlayWeaponAnim(toolType, isPaused);
    }

    protected virtual void OnDisable()
    {
        isEquipped = false;
    }
}