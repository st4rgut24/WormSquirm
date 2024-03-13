using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    // state of weapon use
    public enum AttackState
    {
        Start,  // start attacking
        Attack, // inflicting damage
        Idle    // not in use
    }

    public bool isEquipped = false;
    public ToolType toolType;
    public BoxCollider weaponCollider; // used to trigger attacks

    protected float damage;

    protected Camera playerCamera;
    protected GameObject player;

    protected AttackState attackState;

    protected virtual void Start()
    {
        DisengageWeapon();

        playerCamera = ToolManager.Instance.playerCamera;
        player = ToolManager.Instance.player.gameObject;
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
    /// Start using a weapon
    /// </summary>
    /// <param name="direction">direction of tool</param>
    public virtual void Use()
    {
        attackState = AttackState.Start;
        weaponCollider.enabled = true;
        // damage the gameobject that weapon collider intersects with her
    }

    public void DisengageWeapon()
    {
        weaponCollider.enabled = false;
        attackState = AttackState.Idle;
    }

    protected void DamageCollidedObject(Collider other)
    {
        // check if colliding with any gameobject except its owner
        if (TransformUtils.IsPlayerDamageableObject(other.transform))
        {
            Agent agent = other.gameObject.GetComponent<Agent>();

            Debug.Log("Deal damage to enemy " + other.gameObject.name);
            agent.TakeDamage(damage);
            attackState = AttackState.Attack;
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