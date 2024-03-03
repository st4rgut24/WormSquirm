using System;
using UnityEngine;

public abstract class Ranged: Weapon
{
    RectTransform crosshairUI;

    protected override void Start()
    {
        base.Start();
        crosshairUI = ToolManager.Instance.crosshairUI;
    }

    private void OnEnable()
    {
        crosshairUI.gameObject.SetActive(true);
    }

    /// <summary>
    /// Get direction in which to use the tool
    /// </summary>
    /// <returns>direction</returns>
    public override Vector3 GetDirection()
    {
        // Get the screen position of the crosshair UI element
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(playerCamera, crosshairUI.position);
        return ProjectScreenPosition(screenPos);
    }

    private void OnDisable()
    {
        crosshairUI.gameObject.SetActive(false);
    }
}

