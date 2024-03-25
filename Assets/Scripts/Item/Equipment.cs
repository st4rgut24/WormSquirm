using System;
using UnityEngine;

/// <summary>
/// An item kept in backpack for later use
/// </summary>
public class Equipment : Collectible
{
    [SerializeField]
    private Vector3 EquipOffset;

    [SerializeField]
    private Vector3 LocalScale;

    public override void Collect(Segment segment)
    {
        base.Collect(segment);
        PutBack(); // hide the object in backpack for later use
    }

    /// <summary>
    /// Take out of the backpack so the equipment can be used
    /// </summary>
    public void TakeOut()
    {
        transform.localPosition = EquipOffset;
        transform.localScale = LocalScale;
        gameObject.SetActive(true);
    }

    public void PutBack()
    {
        gameObject.SetActive(false);
    }
}

