using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum PlayerState
{
    Dig,
    Stuck,
    Walk
}

public class MainPlayer : Player
{
    public static event Action MeleeAttackEvent;

    //bool isBlocked;
    List<GameObject> CollidedObstacles;

    public Transform handTransform;
    PlayerState state;

    Controller controller;
    Joystick joystick;

    protected PlayerHealth playerStamina;

    protected override void Awake()
    {
        base.Awake();

        CollidedObstacles = new List<GameObject>();
        health = new PlayerHealth(Consts.HealthSlider, PlayerManager.PlayerHealth);
        playerStamina = new PlayerHealth(Consts.StaminaSlider, PlayerManager.PlayerHealth);
    }

    private void OnEnable()
    {
        Pickaxe.Dig += HandleDig;
    }

    // Roughly estimates the moment of impact when player is attacking
    public void OnMeleeAttack()
    {
        MeleeAttackEvent?.Invoke();
    }

    /// <summary>
    /// listener for when an attack animation is over
    /// </summary>
    public void OnAttackAnimationStopped() {
        ToolManager.Instance.DisengageTool();
    }

    protected override void Start()
    {        
        base.Start();

        state = PlayerState.Dig;

        GameObject joystickGo = GameObject.Find("Variable Joystick");
        joystick = joystickGo.GetComponent<Joystick>();

        controller = new Controller(gameObject, new ChangeSideRotation(ChangeHorizontalRotation), new ChangeMoveDelegate(ChangePlayerMovement));
    }

    protected override void Update()
    {
        base.Update();

        if (joystick.HasInput())
        {
            Vector2 rawInput = joystick.GetInput();
            controller.HandleInput(rawInput);

            playerStamina.ReduceHealth(Consts.ReduceStaminaAmt);
        }
        else
        {
            ChangeMovement(transform.position, false, 0);
            playerStamina.IncreaseHealth(Consts.ReduceStaminaAmt);
        }
    }

    public void EquipTool(GameObject tool)
    {
        tool.transform.parent = handTransform;
    }

    public bool HasStamina()
    {
        return playerStamina.currentHealth > 0;
    }

    public void ChangePlayerMovement(Vector3 destination, bool isContinuous, float speed)
    {
        // adjust the speed based on stamina (get this from player health
        ChangeMovement(destination, isContinuous, speed);
    }

    /// <summary>
    /// Handle digging action
    /// </summary>
    /// <param name="direction">Direction to dig in</param>
    public void HandleDig(Vector3 direction)
    {
        state = PlayerState.Dig;
        // Debug.Log("Start Digging");
        //charAnimator.TriggerAnimation(swingAnimName);
        notifyDig(direction);
    }

    public void PlayWeayponAnimation(ToolType weaponType, bool isPaused)
    {
        playerAnimator.PlayWeaponAnimation(weaponType, isPaused);
    }

    public void StopWeaponAnimation(ToolType weaponType, bool isPaused)
    {
        playerAnimator.StopWeaponAnimation(weaponType, isPaused);
    }

    public void SetCharAnimation(string animName)
    {
        charAnimator.TriggerAnimation(animName);
    }

    private void OnDisable()
    {
        Pickaxe.Dig -= HandleDig;
    }
}

