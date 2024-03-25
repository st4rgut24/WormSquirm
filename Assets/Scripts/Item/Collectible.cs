using System;
using System.Collections.Generic;
using UnityEngine;

public enum CollectibleType
{
    Jewel
}

public abstract class Collectible : MonoBehaviour
{
	CollectibleType type;

	public int value;

    protected bool IsCollected = false;

    private float MagnetizeDist = 3;
    private bool isUnearthed = false; // whether the valuable has been unearthed

    Transform unearther; // transform of the rightful owner of valuable

    /// <summary>
    /// Defines behavior of of item when it is first obtained
    /// </summary>
    /// <param name="segment">Segment in which valuable was discovered</param>
    public virtual void Collect(Segment segment) 
    {
        IsCollected = true;
    } 

    protected virtual void OnEnable()
    {
        SegmentManager.EnterNewSegmentEvent += OnEnterNewSegment;
    }

    private void Update()
    {
        // Move an item that has not been collected yet and is unearthed to nearby player
        if (isUnearthed && !IsCollected && IsMagnetized())
        {
            MoveToRightfulOwner();
        }
    }

    private void MoveToRightfulOwner()
    {
        // Calculate the distance between current position and destination
        float distance = Vector3.Distance(transform.position, unearther.position);

        // Check if the object has reached the destination
        if (distance > 0.01f)
        {
            // Calculate the next position to move towards
            Vector3 newPosition = Vector3.Lerp(transform.position, unearther.position, Consts.MagnetizeSpeed * Time.deltaTime);

            // Move the object towards the destination
            transform.position = newPosition;
        }
    }

    bool IsMagnetized()
    {
        return Vector3.Distance(unearther.position, transform.position) < MagnetizeDist;
    }

    /// <summary>
    /// Check if transform is in range of valuable
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="range">how close transform needs to be in order for valuable to track proximity of transform</param>
    /// <returns>true if in range of transform</returns>
    bool IsInRange(Transform transform, float range)
    {
        return Vector3.Distance(transform.position, transform.position) < range;
    }

    public void OnEnterNewSegment(Transform transform, Segment segment)
    {
        if (transform.CompareTag(Consts.MainPlayerTag) && IsInRange(transform, segment.length))
        {
            // determine spawning logic based off difficulty
            unearther = transform;
            isUnearthed = true;
        }
    }

    protected virtual void OnDisable()
    {
        SegmentManager.EnterNewSegmentEvent -= OnEnterNewSegment;
    }
}

