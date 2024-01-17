using UnityEngine;
using System.Collections;

public abstract class Firearm : Tool
{
    RectTransform crosshairUI;

    private void OnEnable()
    {
        crosshairUI.gameObject.SetActive(true);
    }

    protected override void Start()
    {
        base.Start();
        crosshairUI = ToolManager.Instance.crosshairUI;
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

