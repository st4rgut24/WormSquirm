using UnityEngine;

public class CameraAnchor : MonoBehaviour
{
    // Reference to the RectTransform of the UI element
    public RectTransform uiElementRectTransform;

    // Offset from the bottom-left corner as a percentage of screen width
    private float offsetXPercentage = 0.35f;
    private float offsetYPercentage = 0.2f;
    private float depthMultipler = 1.5f;

    void Awake()
    {
        //GameObject mainPlayerGo = PlayerManager.Instance.GetMainPlayer();
        //MainPlayer player = mainPlayerGo.GetComponent<MainPlayer>();

        //Camera[] cameras = mainPlayerGo.GetComponentsInChildren<Camera>();
        //Camera mainCamera = cameras[0];

        // Ensure both camera and UI element references are set

        Camera mainCamera = Camera.main;

        if (mainCamera != null && uiElementRectTransform != null)
        {
            // Calculate the width of the camera's viewing area
            float halfWidth = mainCamera.orthographicSize * mainCamera.aspect;

            // Calculate the offset in pixels based on the percentage of screen width
            //float offsetX = halfWidth * offsetXPercentage;
            //float offsetY = mainCamera.orthographicSize * offsetYPercentage;

            float depth = mainCamera.nearClipPlane * depthMultipler;
            Debug.Log("Depth " + depth);

            // the distance horizontally to edge of screen should be proportional to vertical distance
            // from edge of screen
            float horOffsetPct = offsetXPercentage / mainCamera.aspect;

            // Get the camera's viewport position
            Vector3 viewportPosition = new Vector3(horOffsetPct, offsetYPercentage, depth);

            // Convert the viewport position to world space
            Vector3 worldPosition = mainCamera.ViewportToWorldPoint(viewportPosition);

            // Apply the offset
            //worldPosition += new Vector3(offsetX, offsetY, 0);

            // Set the UI element's position in world space
            uiElementRectTransform.position = worldPosition;
        }
    }
}
