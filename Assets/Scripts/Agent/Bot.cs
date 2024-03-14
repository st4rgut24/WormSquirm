using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public abstract class Bot : Automaton
{
    public bool addNoise;

    public BotManager.BotType botType;

    public Transform objective;

    protected float startTime;

    protected abstract void SetObjective();

    protected const string isAttackingAnimName = "isAttacking";

    protected float damage = 0; // can be overriden in child classes

    bool isEnemyColliderTriggered;

    protected virtual void Awake()
    {
        SetObjective();

        height = Consts.BotHeight;
        health = new AgentHealth(BotManager.BotHealth);
        isEnemyColliderTriggered = false;
    }

    protected override void Start()
    {
        base.Start();
        charAnimator = new BotAnimator(animator);

        startTime = Time.time;

        Segment initSegment = this.route.GetInitSegment();

        if (initSegment != null)
        {
            AgentManager.Instance.InitTransformSegmentDict(transform, initSegment);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == Consts.MainPlayerTag)
        {
            isEnemyColliderTriggered = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == Consts.MainPlayerTag)
        {
            isEnemyColliderTriggered = false;
        }
    }

    protected override void PlayFinalDestinationAnim()
    {
        charAnimator.TriggerAnimation(isAttackingAnimName);
    }

    protected override Vector3 GetSpawnLocation()
    {
        return this.route.GetCurrentPosition();
    }

    protected override void Move(Waypoint wp)
    {
        Vector3 destination = wp.position;

        Vector3 moveDir = GetMoveDirection(destination);
        Rotate(moveDir);

        ChangeMovement(destination, true, velocity);
    }

    public override bool initRoute(Route route)
    {
        bool isFirstRoute = base.initRoute(route);
        this.route = route;

        if (isFirstRoute)
        {
            Waypoint CurWP = route.GetCurWaypoint();
            Waypoint NextWP = route.GetNextWaypoint();

            Vector3 initDir = (NextWP.position - CurWP.position).normalized;
            transform.rotation = Quaternion.LookRotation(initDir, Vector3.up);
        }

        return isFirstRoute;
    }

    protected override void Rotate(Vector3 moveDirection)
    {
        // Rotate the bot to face the direction it is moving in
        if (moveDirection != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            // Debug.Log("rotate bot " + transform.rotation);
            ChangeRotation(rotation, Consts.botRotationSpeed);
        }
    }

    /// <summary>
    /// Called when bot is inflicting damage on objective gameobject
    /// This is a trigger for the event added to the attack animation
    /// </summary>
    public void TriggerInflictDamage()
    {
        if (isEnemyColliderTriggered) // check if main player collider was triggered
        {
            Agent attackedAgent = objective.GetComponent<Agent>();
            InflictDamage(damage, attackedAgent);
        }
    }

    public override IEnumerator DieCoroutine()
    {
        AbortMovement(); // stop any ongoing movmement
        charAnimator.TriggerAnimation(Consts.DieAnim);
        Fall();
        DisableCollider();

        yield return new WaitForSeconds(Consts.SecondsToDisappear);
        BotManager.Instance.RemoveBot(this);
    }

    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }
}
