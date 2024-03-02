using UnityEngine;
using System.Collections;

public enum PlayerState
{
    Dig,
    Stuck,
    Walk
}

public class MainPlayer : Player
{
    PlayerState state;

    Controller controller;
    Joystick joystick;

    bool isPositionClamped;

    protected PlayerHealth playerStamina;

    private void Awake()
    {
        health = new PlayerHealth(Consts.HealthSlider, PlayerManager.PlayerHealth);
        playerStamina = new PlayerHealth(Consts.StaminaSlider, PlayerManager.PlayerHealth);
    }

    private void OnEnable()
    {
        Pickaxe.Dig += HandleDig;
    }

    protected override void Start()
    {        
        base.Start();

        state = PlayerState.Dig;

        GameObject joystickGo = GameObject.Find("Variable Joystick");
        joystick = joystickGo.GetComponent<Joystick>();

        controller = new Controller(gameObject, new ChangeSideRotation(ChangeHorizontalRotation), new ChangeMoveDelegate(ChangePlayerMovement));

        isPositionClamped = false;
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
    /// Is the player traveling in a direction that is going out of the segment bounds
    /// </summary>
    /// <param name="position">Player transform</param>
    /// <param name="originalPosition">player's original position</param>
    /// <param name="projectedPosition">player's next position</param>
    /// <returns>true if out of bounds</returns>
    public bool isGoingOutOfBounds(Transform transform, Vector3 originalPosition, Vector3 projectedPosition)
    {
        bool outOfBounds = curSegment.IsOutOfBounds(transform, originalPosition, projectedPosition);
        return outOfBounds;
    }

    /// <summary>
    /// Handle digging action
    /// </summary>
    /// <param name="direction">Direction to dig in</param>
    public void HandleDig(Vector3 direction)
    {
        state = PlayerState.Dig;
        Debug.Log("Start Digging");
        charAnimator.TriggerAnimation(swingAnimName);
        notifyDig(direction);

        //controller.AccelerateInDirection(direction);
    }

    private void OnDisable()
    {
        Pickaxe.Dig -= HandleDig;
    }
}

