using System;

public class Consts
{
    // tags
    public const string EnemyTag = "Enemy";
    public const string PlayerTag = "Player";
    public const string TunnelTag = "tunnel";
    public static readonly string[] ObstacleTags = { EnemyTag };

    // names
    public const string HealthSlider = "HealthBar";
    public const string StaminaSlider = "StaminaBar";

    // transforms
    public const float FullRevolution = 360;

    public const float defaultRotationSpeed = 5f;

    // player movement
    public const float ReduceStaminaAmt = .1f;
    public const float WalkingSpeed = 1f;
    public const float RunningSpeed = 6f;
    public const float playerRotationSpeed = 15;

    // bot moveent
    public const float botRotationSpeed = 5;

    // player animation
    public const string MoveAnim = "speed";
    public const string DieAnim = "isDying";
    public const string SwingAnim = "isSwinging";
    public const string ThrowAnim = "isThrowing";
    public const string ShootAnim = "isShooting";
    public const string ChainAnim = "isChaining";

    // bot animation
    public const string SlamAttackAnim = "isAttacking";
    public const float SecondsToDisappear = 3;
}

