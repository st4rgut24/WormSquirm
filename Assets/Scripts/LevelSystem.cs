using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The difficulty of getting past the checkpoints on the return journey
/// </summary>
public struct GateManagerDifficulty
{
    public Vector3 FinalKeyDirection; // direction to reach the final key, determines the average slope of the path
    public Jewel.Type FinalJewelType;
    public int FinalKeyDist;          // distance to reach the final key

    // Intermediary Gates
    public int GateCount;      // total number of gates (does not include the starting gate)
    public int AvgKeyDist;     // average distance between key location and the gate
    public int KeyAngle;       // angle between segment with gate and the key, a bigger angle means a steeper path, a negative angle means descent (easier), and positive (harder due to avalanche risk)
    public int TollMultiplier; // average premium for paying a toll (multiplies the value of the key)

    // Note that the below probabilities must add up to 1.0f
    public TypeProbability[] GateTypeProbabilities;
    //public float KeyedChance;  // Chance that a gate only accepts a key
    //public float TollChance;     // Chance that a gate accepts a key or a toll
}

/// <summary>
/// The difficulty of the combatants
/// </summary>
public struct BotManagerDifficulty
{
    // the number of pursuing bots should increase toward end of game
    public int StartCount;     // number of active bots at start of game
    public int EndCount;       // number of active bots at end of game
    public float Health;       // health of bots
}

/// <summary>
/// The difficulty of the boulders and other inanimate objects
/// </summary>
public struct RockManagerDifficulty
{
    public int SpeedMultiplier;// What to multiply the rock's starting speed by
    public float Health;       // health of rock before it breaks apart
    public float DigChance;    // Chance of rock creation after a dig action
}

/// <summary>
/// The difficulty of the rewards system
/// </summary>
public struct RewardDifficulty
{
    public float AdHealthAmount;         // Average amount of gold from watching an ad, a percentage of total health between 0 and 1
    public float HealthAmount;           // Average amount of health, a percentage of total health between 0 and 1
    public int AdGoldAmount;           // Average amount of gold from watching an ad
    public int TreasureAmount;         // Average amount of gold from a treasure reward
    public int KillEnemyGold;          // Average amount of gold from killing an enemy
    // below reward probabilities must add up to less than or equal to 1. They are exclusive
    public TypeProbability[] RewardTypeProbabilities;
    public float DigTreasureChance;    // Chance of gold in an unearthed tunnel segent
    public float DigHealthChance;      // Chance of health in an unearthed tunnel segment
    public float DigAdChance;          // Chance of ad in an unearthed tunnel segment, ads pay out in gold or health depending on player state
}

public class LevelSystem
{
    int levelCount = 0;

    Dictionary<int, GameLevel> LevelDict = new Dictionary<int, GameLevel>();

    public LevelSystem()
	{
        // TODO: Initialize the levels
        GameLevel level1 = new GameLevel(
            new GateManagerDifficulty {
                AvgKeyDist = 2,
                GateTypeProbabilities = new TypeProbability[]
                {
                    new TypeProbability((int) GateType.Key, .5f),
                    new TypeProbability((int) GateType.Toll, .5f)
                },
                GateCount = 2,
                TollMultiplier = 1,

                FinalKeyDirection = Vector3.forward,
                FinalKeyDist = 30,
                FinalJewelType = Jewel.Type.Emerald,
                KeyAngle = 30
            },
            new BotManagerDifficulty {
                StartCount = 0,
                EndCount = 1,
                Health = 100
            },
            new RockManagerDifficulty {
                DigChance = .5f,
                Health = 100,
                SpeedMultiplier = 1
            },
            new RewardDifficulty {
                AdGoldAmount = 5,
                AdHealthAmount = .1f,
                RewardTypeProbabilities = new TypeProbability[]
                {

                },
                DigAdChance = .2f,
                DigHealthChance = .2f,
                DigTreasureChance = .2f,
                HealthAmount = .1f,
                KillEnemyGold = 5,
                TreasureAmount = 10
            }
        ); ;

        SetLevel(1, level1);
	}

    public void SetLevel(int levelCount, GameLevel level)
    {
        LevelDict[levelCount] = level;
    }

    public GameLevel GetNextLevel()
    {
        levelCount++;
        return LevelDict[levelCount];
    }
}