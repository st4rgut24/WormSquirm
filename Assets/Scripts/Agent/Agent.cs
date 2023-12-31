using UnityEngine;
using System.Collections;
using System;

public class Agent : MonoBehaviour
{
    public static event Action<Transform> OnMove;

    Vector3 prevPosition = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

    Vector3 startNotifyPosition;

    // Use this for initialization
    protected virtual void Start()
	{
        startNotifyPosition = transform.position;
    }

    // Update is called once per frame
    protected virtual void Update()
	{
        float distance = Vector3.Distance(startNotifyPosition, transform.position);

        if (distance >= GameManager.Instance.maxSegmentLength)
        {
            notifyPosition();
            startNotifyPosition = transform.position;
        }
	}

    private void notifyPosition()
    {
        if (transform.position != prevPosition)
        {
            prevPosition = transform.position;
            OnMove?.Invoke(transform);
        }
    }
}

