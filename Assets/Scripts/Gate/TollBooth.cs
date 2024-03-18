using UnityEngine;
using System.Collections;

public class TollBooth : Gate
{
    private const int TollMultiplier = 2; // Determines how expensive the toll will be based on value of key
    private int toll; // toll is an alternative way to open the gate using currency instead of a key

    public override void SetKey(Jewel key)
    {
        base.SetKey(key);
        toll = key.value * TollMultiplier;
    }

    public int GetToll()
    {
        return toll;
    }

    public void PayToll(Inventory inventory)
    {
        if (inventory.Pay(toll))
        {
            Open();
        }
        else
        {
            Debug.LogWarning("Not enough money to pay for the toll");
        }
    }
}

