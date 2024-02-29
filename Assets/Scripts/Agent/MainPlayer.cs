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

        controller = new Controller(gameObject, new ChangeSideRotation(ChangeHorizontalRotation), new ChangeMoveDelegate(ChangeMovement));

        isPositionClamped = false;
    }

    protected override void Update()
    {
        base.Update();

        if (joystick.HasInput())
        {
            Vector2 rawInput = joystick.GetInput();
            controller.HandleInput(rawInput);
        }
        else
        {
            ChangeMovement(transform.position, false, 0);
        }
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

