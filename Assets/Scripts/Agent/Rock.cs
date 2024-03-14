using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

/// <summary>
/// A rolling type of agent
/// </summary>
public class Rock : Automaton
{
    // damage dealt to another object on collision
    const float damage = 1;
    // damage incurred when damaging another object
    float incurredDamage = RockManager.RockHealth; // TODO: temporary just for testing self destruct on contact mechanism

    bool IsWhole = true; // whether the rock is a whole piece or in the process of self destructing

    MeshRenderer WholeBoulderRenderer;

    // contains the split fragments of the boulder, displayed on destruction
    GameObject SplitBoulderParent;

    private void Awake()
    {
        health = new AgentHealth(RockManager.RockHealth);

        SplitBoulderParent = transform.Find(Consts.SplitBoulder).gameObject;

        WholeBoulderRenderer = GetComponent<MeshRenderer>();
    }

    protected override void Update()
    {
        transform.rotation = this.targetRotation;
    }

    // The rock should destroy an agent it comes into contact with
    private void OnTriggerEnter(Collider other)
    {
        //Transform ancestor = TransformUtils.GetAncestorMatchTag(other.transform, Consts.EnemyTag);
        DamageCollidedObject(other);
    }

    protected void DamageCollidedObject(Collider other)
    {
        if (TransformUtils.IsAnyDamageableObject(other.transform))
        {
            Agent agent = other.gameObject.GetComponent<Agent>();

            Debug.Log("Deal damage to enemy " + other.gameObject.name);
            agent.TakeDamage(damage);

            this.TakeDamage(incurredDamage);
        }
    }

    /// <summary>
    /// When the boulder 'dies', it should split into fragments, by removing its
    /// whole version and replacing it with a split version
    /// </summary>
    /// <returns></returns>
    public override IEnumerator DieCoroutine()
    {
        if (IsWhole)
        {
            IsWhole = false;

            DisableCollider();
            WholeBoulderRenderer.enabled = false;

            SplitBoulderParent.SetActive(true); // reveal the border fragments
            yield return new WaitForSeconds(Consts.BoulderDestructTimer);
            GameObject.Destroy(gameObject);
        }
    }

    /// <summary>
    /// When rock reaches final destination it's supposed to split up, and disappear
    /// </summary>
    protected override void PlayFinalDestinationAnim()
    {
        StartCoroutine(DieCoroutine());
    }

    protected override bool IsReachedFinalDestination(Waypoint finalWP)
    {
        return transform.position.Equals(finalWP.position);
    }

    protected override void ReachDestination()
    {
        AbortMovement();
    }

    protected override void Rotate(Vector3 moveDirection)
    {
        // Rotate the bot to face the direction it is moving in
        if (moveDirection != Vector3.zero)
        {
            Debug.Log("Rotate about move direction " + moveDirection);
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, moveDirection);

            // Calculate the target rotation based on the rotation axis and speed
            Quaternion turnRotation = Quaternion.AngleAxis(Consts.rockRotationSpeed * Time.deltaTime, rotationAxis);
            Quaternion targetRotation = turnRotation * transform.rotation;

            //transform.rotation = targetRotation;
            ChangeRotation(targetRotation, Consts.rockRotationSpeed);
        }
    }

    protected override void Move(Waypoint wp)
    {
        Vector3 destination = wp.position;

        Vector3 moveDir = GetMoveDirection(destination);
        Rotate(moveDir);

        ChangeMovement(destination, true, velocity);
    }

    protected override Vector3 GetSpawnLocation()
    {
        return curSegment.GetCenterLineCenter();
    }
}

