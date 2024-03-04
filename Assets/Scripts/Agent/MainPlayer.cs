using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerState
{
    Dig,
    Stuck,
    Walk
}

public class MainPlayer : Player
{
    //bool isBlocked;
    List<GameObject> CollidedObstacles;

    public Transform handTransform;
    PlayerState state;

    Controller controller;
    Joystick joystick;

    protected PlayerHealth playerStamina;

    private void Awake()
    {
        //isBlocked = false;
        CollidedObstacles = new List<GameObject>();
        health = new PlayerHealth(Consts.HealthSlider, PlayerManager.PlayerHealth);
        playerStamina = new PlayerHealth(Consts.StaminaSlider, PlayerManager.PlayerHealth);
    }

    private void OnEnable()
    {
        Pickaxe.Dig += HandleDig;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (TransformUtils.IsTransformMatchTags(other.transform, Consts.ObstacleTags))
        {
            // want to add the root parent
            CollidedObstacles.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Debug.Log("Obstacle avoided");
        RemoveCollidedObject(other.gameObject);
    }

    public void RemoveCollidedObject(GameObject hitObject)
    {
        if (CollidedObstacles.Contains(hitObject))
        {
            Debug.Log("Remove obstacle " + hitObject.name);
            CollidedObstacles.Remove(hitObject);
        }
    }

    public bool IsBlocked()
    {
        return CollidedObstacles.Count > 0;
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
        if (!IsBlocked())
        {
            ChangeMovement(destination, isContinuous, speed);
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
        // Debug.Log("Start Digging");
        //charAnimator.TriggerAnimation(swingAnimName);
        notifyDig(direction);
    }

    public void SetWeaponAnimation(ToolType weaponType)
    {
        playerAnimator.TriggerWeaponAnimation(weaponType);
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

