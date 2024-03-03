using System;
using UnityEngine;

public abstract class Melee: Weapon
{
    Vector3 meleeStartPos;
    Vector3 meleeEndPos;

    void Update()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonUp(1)) // 0 represents the left mouse button
        {
            Vector3 mousePosition = Input.mousePosition;

            meleeEndPos = new Vector2(mousePosition.x, mousePosition.y);
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
                    meleeEndPos = touch.position;
                    Use();
                    break;
            }
        }
        #endif
    }

    /// <summary>
    /// Convert screen coords to world pos
    /// </summary>
    /// <returns>world coords</returns>
    /// <exception cref="Exception"></exception>
    public override Vector3 GetDirection()
    {
        if (meleeEndPos.Equals(DefaultUtils.DefaultVector3))
        {
            throw new Exception("Not a valid direction");
        }
        else
        {
            return ProjectScreenPosition(meleeEndPos);
        }
    }
}

