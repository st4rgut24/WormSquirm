using System;
using UnityEngine;

public abstract class Melee: Weapon
{
    bool IsTouchBeginInMeleeCanvas = false;

    Vector3 meleeStartPos;
    Vector3 meleeEndPos;

    public bool IsTouchInsideMeleeCanvas(Vector3 screenPos)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(ToolManager.Instance.MeleeCanvas, screenPos);
    }

    void Update()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonUp(1) && IsTouchInsideMeleeCanvas(Input.mousePosition)) // 0 represents the left mouse button
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
                    IsTouchBeginInMeleeCanvas = IsTouchInsideMeleeCanvas(touch.position);
                    break;

                case TouchPhase.Moved:
                    break;

                case TouchPhase.Ended:
                    // Use melee weapon if the touches were made within permissable area
                    if (IsTouchBeginInMeleeCanvas && IsTouchInsideMeleeCanvas(touch.position)) {
                        IsTouchBeginInMeleeCanvas = false; // reset variable for next touch
                        meleeEndPos = touch.position;
                        Use();
                    }
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

