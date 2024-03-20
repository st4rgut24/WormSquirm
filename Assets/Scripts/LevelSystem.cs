using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// The difficulty of the trip
/// </summary>
public struct RouteDifficulty
{
    float Distance;     // Distance to final key
    float Direction;    // Direction to reach the final key (determine steepness of route)
}

/// <summary>
/// The difficulty of getting past the checkpoints on the return journey
/// </summary>
public struct GateManagerDifficulty
{
    int KeyedCount;     // nuber of gates which only accept keys
    int TollCount;      // number of toll gates which accept currency or key
    int AvgKeyDist;     // average distance between key location and player
}

/// <summary>
/// The difficulty of the combatants
/// </summary>
public struct BotManagerDifficulty
{
    // the number of pursuing bots should increase toward end of game
    int StartCount;     // number of active bots at start of game
    int EndCount;       // number of active bots at end of game
    float Health;       // health of bots
}

/// <summary>
/// The difficulty of the boulders and other inanimate objects
/// </summary>
public struct RockManagerDifficulty
{
    int SpeedMultiplier;// What to multiply the rock's starting speed by
    float Health;       // health of rock before it breaks apart
    float DigChance;    // Chance of rock creation after a dig action
}

/// <summary>
/// The difficulty of the rewards system
/// </summary>
public struct RewardDifficulty
{
    float DigTreasureChance;    // Chance of gold in an unearthed tunnel segent
    float DigHealthChance;      // Chance of health in an unearthed tunnel segment
    float DigAdChance;          // Chance of ad in an unearthed tunnel segment, ads pay out in gold or health depending on player state
    int AdGoldAmount;           // Average amount of gold from watching an ad
    int AdHealthAmount;         // Average amount of gold from watching an ad
    int TreasureAmount;         // Average amount of gold from a treasure reward
    int HealthAmount;           // Average amount of health
    int KillEnemyGold;          // Average amount of gold from killing an enemy
}

public class LevelSystem
{
    int levelCount = 0;

    Dictionary<int, Level> LevelDict; 

    public LevelSystem()
	{
        // TODO: Initialize the levels
	}

    public Level GetNextLevel()
    {
        levelCount++;
        return LevelDict[levelCount];
    }
}

