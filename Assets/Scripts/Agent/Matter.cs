using UnityEngine;
using System.Collections;

public class Matter : MonoBehaviour
{
    public Segment curSegment;
    protected BoxCollider agentCollider;

    protected virtual void Awake()
    {
        agentCollider = GetComponent<BoxCollider>();
    }

    public void SetCurSegment(Segment segment)
    {
        curSegment = segment;
    }

    public Vector3 GetClosestCenterPoint()
    {
        return curSegment.GetClosestPointToCenterline(transform.position);
    }
}

