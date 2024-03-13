using UnityEngine;
using System.Collections;
using UnityEditor;

public class ColliderPreventOverlap : MonoBehaviour
{
    public Collider otherCollider;

    void OnTriggerStay(Collider other)
    {
        // Check if the overlapping collider is the one we want to prevent overlapping with
        if (TransformUtils.IsTransformMatchTags(other.transform, Consts.ObstacleTags))
        {
            // Calculate the direction to move this collider to prevent overlap
            Vector3 direction = transform.position - other.transform.position;
            float distance = direction.magnitude;

            // Move this collider away from the other collider to prevent overlap
            transform.position += direction.normalized * (distance - 0.1f); // 0.1f is just an arbitrary value to ensure a small separation
        }
    }
}


