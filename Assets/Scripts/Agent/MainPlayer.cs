using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using DanielLochner.Assets.SimpleScrollSnap;

public enum PlayerState
{
    Dig,
    Stuck,
    Walk
}

public class MainPlayer : Player
{
    Inventory inventory;
    Detector detector;

    public static event Action MeleeAttackEvent;

    //bool isBlocked;
    List<GameObject> CollidedObstacles;

    public Transform handTransform;
    public Transform leftHandTransform;

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
        detector = GameObject.Find(Consts.Detector).GetComponent<Detector>();
        inventory = GetComponentInChildren<Inventory>();

        inventory.SetMainPlayer(this);
        detector.Init(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Pickaxe.Dig += HandleDig;
        Jewel.CollectJewelEvent += inventory.OnKeyFound;
        GateManager.CreateKeyEvent += inventory.OnKeyCreated;
        GateManager.DestroyKeyEvent += inventory.OnKeyDestroyed;
        Detector.DetectDistanceEvent += inventory.OnDistanceToItemReceived;
        SimpleScrollSnap.SelectedItemEvent += inventory.OnSelectedItem;
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

    /// <summary>
    /// Logic for collecting items goes here
    /// </summary>
    /// <param name="other">collided with object</param>
    private void OnTriggerEnter(Collider other)
    {
        if (TransformUtils.IsCollectible(other.transform))
        {
            Equipment valuable = other.GetComponent<Equipment>();
            valuable.Collect(curSegment);
        }
    }

    public void EquipValuable(GameObject obj)
    {
        obj.transform.parent = leftHandTransform;
    }

    public void EquipTool(GameObject obj)
    {
        obj.transform.parent = handTransform;
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

    protected override void OnDisable()
    {
        base.OnDisable();
        Pickaxe.Dig -= HandleDig;
        Jewel.CollectJewelEvent -= inventory.OnKeyFound;
        GateManager.CreateKeyEvent -= inventory.OnKeyCreated;
        GateManager.DestroyKeyEvent -= inventory.OnKeyDestroyed;
        Detector.DetectDistanceEvent -= inventory.OnDistanceToItemReceived;
        SimpleScrollSnap.SelectedItemEvent -= inventory.OnSelectedItem;
    }
}

