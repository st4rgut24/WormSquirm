using UnityEngine;

public class CameraAnchor : MonoBehaviour
{
    // Reference to the RectTransform of the UI element
    public RectTransform uiElementRectTransform;

    // Offset from the bottom-left corner as a percentage of screen width
    [SerializeField]
    private float offsetXPercentage;

    [SerializeField]
    private float offsetYPercentage;
    private float depthMultipler = 1.5f;

    void Awake()
    {
        // Ensure both camera and UI element references are set

        Camera mainCamera = Camera.main;

        if (mainCamera != null && uiElementRectTransform != null)
        {
            // Calculate the width of the camera's viewing area
            float halfWidth = mainCamera.orthographicSize * mainCamera.aspect;

            float depth = mainCamera.nearClipPlane * depthMultipler;

            // the distance horizontally to edge of screen should be proportional to vertical distance
            // from edge of screen
            float horOffsetPct = offsetXPercentage / mainCamera.aspect;

            // Get the camera's viewport position
            Vector3 viewportPosition = new Vector3(horOffsetPct, offsetYPercentage, depth);

            // Convert the viewport position to world space
            Vector3 worldPosition = mainCamera.ViewportToWorldPoint(viewportPosition);

            // Set the UI element's position in world space
            uiElementRectTransform.position = worldPosition;
        }
    }
}
