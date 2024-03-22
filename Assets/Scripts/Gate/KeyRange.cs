using UnityEngine;

/// <summary>
/// Range of angles that are acceptable.
/// Stores two pairs of range bounds, reflecting the provided bounds across y axis
/// </summary>
public class KeyRange
{
	public struct Range
	{
		public float minAngle;
        public float maxAngle;
    }

    public Range range { get; private set; }
    public Range reflectedRange { get; private set; }

    /// <summary>
    /// Get a random angle within the ranges
    /// </summary>
    /// <returns>angle</returns>
    public float GetAngle()
    {
        bool isReflected = RandomUtils.CoinToss();

        Range chosenRange = isReflected ? reflectedRange : range;
        float randRangedAngle = RandomUtils.GetRandomNumber(chosenRange.minAngle, chosenRange.maxAngle);

        return randRangedAngle;
    }

    /// <summary>
    /// Validate that the range can be reflected across Y axis
    /// Invalid if 
    /// </summary>
    private void ValidateRange(float minAngle, float maxAngle)
    {
        if (minAngle > maxAngle || minAngle < Consts.SpawnKeyAngleMin || maxAngle > Consts.SpawnKeyAngleMax)
        {
            throw new System.Exception("The provided spawn key angle range is invalid");
        }
    }

    private Range reflectRange(Range range)
    {
        float minReflectedRange = 180 - range.minAngle;
        float maxReflectedRange = 180 - range.maxAngle;

        return new Range { minAngle = minReflectedRange, maxAngle = maxReflectedRange };
    }

    public KeyRange(float minAngle, float maxAngle)
	{
        ValidateRange(minAngle, maxAngle);

        range = new Range { minAngle = minAngle, maxAngle = maxAngle };
        reflectedRange = reflectRange(range);
	}
}

