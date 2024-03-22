using System;

public class Consts
{
    // Game Manager
    public const int MinSetupWPs = 3;

    // tags
    public const string JewelTag = "Jewel";

    public const string PickaxeTag = "pickaxe";
    public const string CrossbowTag = "crossbow";
    public const string MaceTag = "mace";

    public const string MainCameraTag = "MainCamera";

    public const string EnemyTag = "Enemy";
    public const string RockTag = "Rock";
    public const string PlayerTag = "Player";
    public const string MainPlayerTag = "MainPlayer";
    public const string TunnelTag = "tunnel";
    public const string DiggerBotTag = "Digibot";
    public const string GateTag = "Gate";
    public const string TollBoothTag = "Toll";

    public static readonly string[] PlayerTags = { PlayerTag, MainPlayerTag };
    public static readonly string[] EnemyTags = { EnemyTag };
    public static readonly string[] PlayerDamageableTags = { EnemyTag, RockTag, PlayerTag }; // main player tag is ommitted because cannot damage self
    public static readonly string[] AllDamageableTags = { EnemyTag, RockTag, PlayerTag, MainPlayerTag };
    public static readonly string[] LivingAgentTags = { EnemyTag, PlayerTag, MainPlayerTag };
    public static readonly string[] ObstacleTags = { EnemyTag, PlayerTag, MainPlayerTag, GateTag, TollBoothTag };
    public static readonly string[] ItemTags = { JewelTag };

    // names
    public const string HealthSlider = "HealthBar";
    public const string StaminaSlider = "StaminaBar";
    public const string Arrow = "arrow";
    public const string SplitBoulder = "split_parent";

    // transforms
    public const float FullRevolution = 360;

    public const float defaultRotationSpeed = 5f;

    public const float FallSpeed = 3;

    public const float MagnetizeSpeed = 5;

    // player movement
    public const float ReduceStaminaAmt = .05f;
    public const float WalkingSpeed = 3f;
    public const float RunningSpeed = 6f;
    public const float playerRotationSpeed = 15;
    public const float PlayerHeight = 2.4f;

    // bot movement
    public const float botRotationSpeed = 5;
    public const float rockRotationSpeed = 100;
    public const float randomMoveRange = 2;
    public const float BotHeight = 2.4f;
    public const float IdleSpeed = 0;

    // player animation
    public const string MoveAnim = "speed";
    public const string DieAnim = "isDying";
    public const string SwingAnim = "overheadSwing";
    public const string ThrowAnim = "isThrowing";
    public const string ChainAnim = "rightHandSwing";
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
    public const float MaxDistToDig = 3; // the maximum distance player can be from tunnel walls to dig
    public const float MinDistToEndCap = 2.5f; // the closest a player can get to the end of a tunnel
    public const int DistFromNewTunnelEnd = 12; // how many units away from end of a newly created tunnel a player is
}

