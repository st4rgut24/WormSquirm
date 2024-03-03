using System;

public class Consts
{
    // tags
    public const string PlayerTag = "Player";
    public const string TunnelTag = "tunnel";

    // names
    public const string HealthSlider = "HealthBar";
    public const string StaminaSlider = "StaminaBar";

    // transforms
    public const float FullRevolution = 360;
    public const float rotationSpeed = 5;

    // player movement
    public const float ReduceStaminaAmt = .1f;
    public const float WalkingSpeed = 1f;
    public const float RunningSpeed = 6f;

    // player animation
    public const string MoveAnim = "speed";
    public const string DieAnim = "isDying";
    public const string SwingAnim = "isSwinging";
    public const string ThrowAnim = "isThrowing";
    public const string ShootAnim = "isShooting";
    public const string ChainAnim = "isChaining";

    // bot animation
    public const string SlamAttackAnim = "isAttacking";
}

