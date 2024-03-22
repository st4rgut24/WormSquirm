using System;

/// <summary>
/// Assigns a probability to a type
/// </summary>
public class TypeProbability
{
    public int enumType;
    public float prob;

    public TypeProbability(int enumType, float probability)
    {
        this.enumType = enumType;
        this.prob = probability;
    }
}

public class RandomUtils
{

    /// <summary>
    /// Confirms a list of floats adds to 1
    /// </summary>
    /// <param name="probabilities">list of floats from 0 to 1</param>
    /// <exception cref="ArgumentException"></exception>
    public static void ValidateSumOfProbabilities(TypeProbability[] probabilities)
    {
        // Check if probabilities array sums up to approximately 1
        double sum = 0;
        foreach (TypeProbability typeProbability in probabilities)
        {
            sum += typeProbability.prob;
        }

        if (Math.Abs(sum - 1.0) > 0.000001)
        {
            throw new ArgumentException("Probabilities do not sum up to 1.");
        }

    }

    /// <summary>
    /// Use a cumulative probability approach to pick an type from a list of probabilities
    /// </summary>
    /// <param name="probabilities">the list of probabilites</param>
    /// <returns></returns>
    public static TypeProbability PickTypeProbability(TypeProbability[] probabilities)
    {
        ValidateSumOfProbabilities(probabilities);

        Random random = new Random();

        // Generate a random number between 0 and 1
        double randomNumber = random.NextDouble();

        // Calculate cumulative probabilities
        double cumulativeProbability = 0;
        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulativeProbability += probabilities[i].prob;
            if (randomNumber < cumulativeProbability)
            {
                return probabilities[i]; // Return the index when cumulative probability exceeds the random number
            }
        }

        // In case of precision errors, return the last index
        return probabilities[probabilities.Length - 1];
    }
}

