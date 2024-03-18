using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class ColliderPreventOverlap : MonoBehaviour
{
    public static event Action<Vector3> CollideEvent;

    public Collider otherCollider;
    public float separateDistance;

    Agent agent;

    private void Start()
    {
        agent = gameObject.GetComponent<Agent>();
    }

    void OnTriggerStay(Collider other)
    {
        // Check if the overlapping collider is the one we want to prevent overlapping with
        if (TransformUtils.IsTransformMatchTags(other.transform, Consts.ObstacleTags))
        {

            // Calculate the direction to move this collider to prevent overlap
            Matter tunnelObject = other.transform.gameObject.GetComponent<Matter>();

            // because the pivot of the gameobject is not the center of the tunnel,
            // subtracting the agents positions will misalign the player wrt to tunnel

            // use tunnel centers to align direction with slope of tunnel
            Vector3 agentCenterPoint = agent.GetClosestCenterPoint();
            Vector3 enemyCenterPoint = tunnelObject.GetClosestCenterPoint();

            Vector3 direction = (agentCenterPoint - enemyCenterPoint).normalized;
            Vector3 translation = direction * separateDistance;

            Vector3 projectedPosition = transform.position + translation;

            if (agent.isGoingOutOfBounds(agent.transform, agent.transform.position, projectedPosition))
            {
                return;
            }
            else
            {
                // Move this collider away from the other collider to prevent overlap
                transform.position = projectedPosition; // 0.1f is just an arbitrary value to ensure a small separation
            }
        }
    }
}


