using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class Ranged : Weapon
{
    [SerializeField]
    protected Animator animator;

    [SerializeField]
    private Transform projectileTransform;

    protected int animShootFrame; // frame to freeze the animation while shooting to prevent sway
    private bool hasInputBeenProcessed = false;

    GameObject mainCameraGo;
    GameObject shootCameraGo;

    public Button ExitShootCameraButton;
    public RawImage ExitShootCameraImage;

    bool isSniping = false;

    private void OnEnable()
    {
        if (isEquipped)
        {
            // as soon as the weapon is equipped / active show the player carrying the weapon animation
            PlayWeaponAnim(isSniping);
        }
    }
    protected override void Start()
    {
        base.Start();
        mainCameraGo = GameObject.FindGameObjectWithTag(Consts.MainCameraTag);
        shootCameraGo = transform.GetComponentInChildren<Camera>().gameObject;

        GameObject ExitRangedViewGo = GameObject.Find("ExitRangedView");

        ExitShootCameraButton = ExitRangedViewGo.GetComponent<Button>();
        ExitShootCameraButton.onClick.AddListener(HandleExitRangedView);

        ExitShootCameraImage = ExitRangedViewGo.GetComponent<RawImage>();
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonUp(1) && IsTouchInsideWeaponCanvas(Input.mousePosition)) // 0 represents the left mouse button
        {
            Vector3 mousePosition = Input.mousePosition;
            Use();
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // Consider the first touch for simplicity

            if (touch.phase == TouchPhase.Began && IsTouchInsideWeaponCanvas(touch.position))
            {
                Use();

            }
        }
#endif
    }

    public override bool GetPauseAnimStatus()
    {
        return isSniping;
    }

    private void HandleExitRangedView()
    {
        ExitSniperMode();
    }

    public override void Use()
    {
        base.Use();
        // switch to the shooting camera if the main camera is in use
        if (Camera.main && Camera.main.enabled)
        {
            EnableSniperMode();
        }
        else
        {
            // ranged camera is already enabled, fire the weapon
            FireWeapon();
        }
    }

    private void FireWeapon()
    {
        Vector3 direction = GetDirection();
        // shoot the ray in the direction and see if it hits anything
        animator.SetTrigger(Consts.FireWeaponAnim);


        Collider collider = ShootRay(direction);

        if (collider != null)
        {
            DamageCollidedObject(collider);
        }
    }

    public void ExitSniperMode()
    {
        StopWeaponAnim(isSniping);
        SwapOutRangedCamera();
        ExitShootCameraButton.enabled = false;
        ExitShootCameraImage.enabled = false;
        isSniping = false;
    }

    public void EnableSniperMode()
    {
        isSniping = true;
        SwapInRangedCamera();
        ExitShootCameraButton.enabled = true;
        ExitShootCameraImage.enabled = true;

        PlayWeaponAnim(isSniping); // freeze position in sniper mode
    }

    public void SwapInRangedCamera()
    {
        shootCameraGo.GetComponent<Camera>().enabled = true;
        mainCameraGo.GetComponent<Camera>().enabled = false;
    }

    public void SwapOutRangedCamera()
    {
        mainCameraGo.GetComponent<Camera>().enabled = true;
        shootCameraGo.GetComponent<Camera>().enabled = false;
    }

    protected Collider ShootRay(Vector3 direction)
    {
        Ray ray = new Ray(transform.position, direction);
        Debug.DrawRay(transform.position, direction * 10, Color.red, 100);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit object has a collider and is set to trigger
            return hit.collider;
        } else
        {
            return null;
        }
    }

    /// <summary>
    /// Get direction in which to use the tool
    /// </summary>
    /// <returns>direction</returns>
    public override Vector3 GetDirection()
    {
        return -projectileTransform.up;
    }

    protected override void OnDisable()
    {
        if (isEquipped) // check if weapon was equipped
        {
            // the weapon animation will be unpaused when this component is disabled, as the only way to disable it is to exit the snipe mode
            // which pauses the animation
            StopWeaponAnim(isSniping); // when the weapon is deactivated stop whatever carrying animation associated with ranged weapon
        }

        base.OnDisable();
    }
}

