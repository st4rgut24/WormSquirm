using System;
using System.Collections.Generic;
using UnityEngine;

public class Pickaxe : Tool
{
    public static event Action<Vector3> Dig;

    Vector2 digDirection = DefaultUtils.DefaultVector3;


    void Update()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonUp(1)) // 0 represents the left mouse button
        {
            Vector3 mousePosition = Input.mousePosition;
            digDirection = new Vector2(mousePosition.x, mousePosition.y);
            Use();
        }
        #endif

        #if UNITY_ANDROID || UNITY_IOS
        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // Consider the first touch for simplicity

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    break;

                case TouchPhase.Moved:
                    break;

                case TouchPhase.Ended:
                    digDirection = touch.position;
                    Use();
                    break;
            }
        }
        #endif
    }


    public override void Use()
    {
        Vector3 direction = GetDirection();
        Dig?.Invoke(direction);
    }

    public override Vector3 GetDirection()
    {
        if (digDirection.Equals(DefaultUtils.DefaultVector3))
        {
            throw new Exception("Not a valid direction");
        }
        else
        {
            return ProjectScreenPosition(digDirection);
        }
    }
}

