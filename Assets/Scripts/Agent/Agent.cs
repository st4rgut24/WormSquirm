using UnityEngine;
using System.Collections;
using System;

public class Agent : MonoBehaviour
{
    public static event Action<Transform> OnMove;

    Vector3 prevPosition = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

    // Use this for initialization
    protected virtual void Start()
	{
        StartCoroutine(NotifyCoroutine());
    }

    // Update is called once per frame
    void Update()
	{
			
	}

    private IEnumerator NotifyCoroutine()
    {
        while (true)
        {
            if (transform.position != prevPosition)
            {
                prevPosition = transform.position;
                OnMove?.Invoke(transform);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}

