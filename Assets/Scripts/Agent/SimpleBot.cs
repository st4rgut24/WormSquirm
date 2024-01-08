using System;
using UnityEngine;

public class SimpleBot : Bot
{
    protected override void SetObjective()
    {
        GameObject simpObj = GameObject.Find("SimpObjective");
        objective = simpObj.transform;
    }
}

