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

        controller = new Controller(gameObject, new ChangeRotationDelegate(ChangeRotation), new ChangeMoveDelegate(ChangeMovement));

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
    /// Is the player outside the bounds of the current segment
    /// </summary>
    /// <param name="transform">player's transform</param>
    /// <param name="position">vector3 position</param>
    /// <returns>true if out of bounds</returns>
    public bool isOutOfBounds(Transform transform, Vector3 position)
    {
        return curSegment.isOutOfBounds(transform, position);
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

