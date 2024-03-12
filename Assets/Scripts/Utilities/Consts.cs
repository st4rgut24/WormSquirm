using System;

public class Consts
{
    // tags
    public const string PickaxeTag = "pickaxe";
    public const string CrossbowTag = "crossbow";
    public const string ChainTag = "chain";

    public const string MainCameraTag = "MainCamera";

    public const string EnemyTag = "Enemy";
    public const string RockTag = "Rock";
    public const string PlayerTag = "Player";
    public const string MainPlayerTag = "MainPlayer";
    public const string TunnelTag = "tunnel";

    public static readonly string[] PlayerTags = { PlayerTag, MainPlayerTag };
    public static readonly string[] ObstacleTags = { EnemyTag };
    public static readonly string[] PlayerDamageableTags = { EnemyTag, RockTag, PlayerTag }; // main player tag is ommitted because cannot damage self
    public static readonly string[] AllDamageableTags = { EnemyTag, RockTag, PlayerTag, MainPlayerTag };

    // names
    public const string HealthSlider = "HealthBar";
    public const string StaminaSlider = "StaminaBar";
    public const string Arrow = "arrow";
    public const string SplitBoulder = "split_parent";

    // transforms
    public const float FullRevolution = 360;

    public const float defaultRotationSpeed = 5f;

    // player movement
    public const float ReduceStaminaAmt = .05f;
    public const float WalkingSpeed = 2f;
    public const float RunningSpeed = 6f;
    public const float playerRotationSpeed = 15;

    // bot movement
    public const float botRotationSpeed = 5;
    public const float rockRotationSpeed = 100;
    public const float randomMoveRange = 2;

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

    // timers
    public const float BoulderDestructTimer = 2;

    // tunnels
    public const float IntersectionOffset = 1; // intended to create an overlap between interecting segments to hide void in between
}

