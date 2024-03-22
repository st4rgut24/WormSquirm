using System;

public class GameLevel
{
    public GateManagerDifficulty gateManagerDifficulty;
    public BotManagerDifficulty botManagerDifficulty;
    public RockManagerDifficulty rockManagerDifficulty;
    public RewardDifficulty rewardDifficulty;

    public GameLevel(GateManagerDifficulty gateManagerDifficulty, BotManagerDifficulty botManagerDifficulty, RockManagerDifficulty rockManagerDifficulty, RewardDifficulty rewardDifficulty)
	{
        this.gateManagerDifficulty = gateManagerDifficulty;
        this.botManagerDifficulty = botManagerDifficulty;
        this.rockManagerDifficulty = rockManagerDifficulty;
        this.rewardDifficulty = rewardDifficulty;
    }
}

