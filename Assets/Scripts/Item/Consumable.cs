using System;
using UnityEngine;

/// <summary>
/// An item that is used as soon as it is obtained
/// </summary>
public class Consumable : Equipment
{
    public override void Collect(Segment segment)
    {
        base.Collect(segment);
        GameObject.Destroy(gameObject);
    }
}