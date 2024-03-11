using System;

public class Consts
{
    // tags
    public const string PickaxeTag = "pickaxe";
    public const string CrossbowTag = "crossbow";
    public const string ChainTag = "chain";

    public const string MainCameraTag = "MainCamera";

    public const string EnemyTag = "Enemy";
    public const string PlayerTag = "Player";
    public const string TunnelTag = "tunnel";
    public static readonly string[] ObstacleTags = { EnemyTag };

    // names
    public const string HealthSlider = "HealthBar";
    public const string StaminaSlider = "StaminaBar";
    public const string Arrow = "arrow";

    // transforms
    public const float FullRevolution = 360;

    public const float defaultRotationSpeed = 5f;

    // player movement
    public const float ReduceStaminaAmt = .1f;
    public const float WalkingSpeed = 1f;
    public const float RunningSpeed = 6f;
    public const float playerRotationSpeed = 15;

    // bot movement
    public const float botRotationSpeed = 5;
    public const float rockRotationSpeed = 100;

    // player animation
    public const string MoveAnim = "speed";
    public const string DieAnim = "isDying";
    public const string SwingAnim = "isSwinging";
    public const string ThrowAnim = "isThrowing";
    public const string ChainAnim = "isChaining";
    public const string ShootAnim = "isShooting";
    public const string SnipeAnim = "isSniping";

    // bot animation
    public const string SlamAttackAnim = "isAttacking";
    public const float SecondsToDisappear = 3;

    // weapon animation
    public const string FireWeaponAnim = "isFire";
    public const string ReloadWeaponAnim = "isFill";

    // test
    public const string BotRouteDrawer = "BotWaypointDrawer";
    public const string RockRouteDrawer = "RockWaypointDrawer";
}

