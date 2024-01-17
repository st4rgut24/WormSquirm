using UnityEngine;
using System.Collections;
using System;
using static UnityEngine.GraphicsBuffer;

public class Agent : MonoBehaviour
{
    public static event Action<Transform, Vector3> OnDig;

    Vector3 prevPosition = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

    public bool isDigging;

    Quaternion targetRotation;
    Vector3 targetPosition;

    Vector3 startNotifyPosition;

    // Use this for initialization
    protected virtual void Start()
	{
        startNotifyPosition = transform.position;
    }

    // Update is called once per frame
    protected virtual void Update()
	{
	}

    protected void notifyDig(Vector3 digDirection)
    {
        if (transform.position != prevPosition)
        {
            Debug.Log("Notify dig. Current position " + transform.position + " Previous position " + prevPosition);
            prevPosition = transform.position;
            OnDig?.Invoke(transform, digDirection);
        }
    }
}

