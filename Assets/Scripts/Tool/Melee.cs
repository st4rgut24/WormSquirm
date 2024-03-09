using System;
using UnityEngine;

public abstract class Melee: Weapon
{
    protected bool IsTouchBeginInWeaponCanvas = false;

    Vector3 meleeStartPos;
    Vector3 meleeEndPos;

    void Update()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonUp(1) && IsTouchInsideWeaponCanvas(Input.mousePosition)) // 0 represents the left mouse button
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
                    IsTouchBeginInWeaponCanvas = IsTouchInsideWeaponCanvas(touch.position);
                    break;

                case TouchPhase.Moved:
                    break;

                case TouchPhase.Ended:
                    // Use melee weapon if the touches were made within permissable area
                    if (IsTouchBeginInWeaponCanvas && IsTouchInsideWeaponCanvas(touch.position)) {
                        IsTouchBeginInWeaponCanvas = false; // reset variable for next touch
                        meleeEndPos = touch.position;
                        Use();
                    }
                    break;
            }
        }
        #endif
    }

    //Upon collision with another GameObject, this GameObject will reverse direction
    private void OnTriggerEnter(Collider other)
    {
        //Transform ancestor = TransformUtils.GetAncestorMatchTag(other.transform, Consts.EnemyTag);
        DamageCollidedObject(other);
    }

    public override bool GetPauseAnimStatus()
    {
        return false; // melee weapons never pause char animations
    }

    public override void Use()
    {
        base.Use();
        // play the melee animation when we are using the tool
        PlayWeaponAnim(false);        
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

