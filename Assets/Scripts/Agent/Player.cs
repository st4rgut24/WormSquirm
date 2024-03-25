using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using static UnityEngine.GraphicsBuffer;

public class Player : Agent
{
    public delegate void ChangeSideRotation(float horRot, float? speed);
    public delegate void ChangeMoveDelegate(Vector3 destination, bool isContinuous, float speed);

    protected PlayerAnimator playerAnimator;
    private float targetElevation;

    protected virtual void OnEnable()
    {
        SegmentManager.EnterNewSegmentEvent += OnEnterNewSegment;
    }

    protected override void Start()
    {
        // Start the coroutine
        base.Start();

        height = Consts.PlayerHeight;
        //string[] allAnimNames = animNames.Concat(agentAnimNames).ToArray();
        playerAnimator = new PlayerAnimator(animator);
        charAnimator = playerAnimator;
        targetElevation = DefaultUtils.DefaultElevation;
    }

    protected override void Update()
    {
        base.Update();

        if (!targetElevation.Equals(DefaultUtils.DefaultElevation))
        {
            float newY = Mathf.Lerp(transform.position.y, targetElevation, Consts.ElevateSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            bool isElevationReached = Mathf.Abs(newY - targetElevation) <= Consts.LerpThreshhold;

            if (isElevationReached)
            {
                Debug.Log("Target elevation reached: " + targetElevation);
                targetElevation = DefaultUtils.DefaultElevation; // set this flag to complete the elevation process and not
                // interfere with future movement actions beyond the entrance of a segment
            }
        }
    }

    public void OnEnterNewSegment(Transform transform, Segment segment)
    {
        Vector3 closestCenterPoint = GetClosestCenterPoint(segment);
        targetElevation = closestCenterPoint.y;
        Debug.Log("Move to targetElevaton of new segment: " + targetElevation);
    }

    /// <summary>
    /// Inflict damage on opponent if in proximity
    /// </summary>
    /// <param name="damage">amount of damage to inflict</param>
    /// <param name="attackedAgent">agent that is attacked</param>
    protected override void InflictDamage(float damage, Agent attackedAgent)
    {
        base.InflictDamage(damage, attackedAgent);
    }

    public override IEnumerator DieCoroutine()
    {
        charAnimator.TriggerAnimation(Consts.DieAnim);
        Fall();
        yield return null;
        // TODO: maybe emit an event to trigger some end screen
    }

    protected virtual void OnDisable()
    {
        SegmentManager.EnterNewSegmentEvent -= OnEnterNewSegment;
    }
}