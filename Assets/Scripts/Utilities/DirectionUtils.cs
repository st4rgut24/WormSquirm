using System;
using UnityEngine;

public class DirectionUtils
{
    /// <summary>
    /// Get a position relative to some transform
    /// </summary>
    /// <param name="transform">The transform of a player</param>
    /// <param name="offsetMultiplier">Units offset from transform</param>
    /// <returns>Heading</returns>
    public static Heading GetHeading(Vector3 origin, Vector3 direction, int offsetMultiplier)
    {
        Vector3 offsetPos = origin + direction * offsetMultiplier;
        return new Heading(offsetPos, direction);
    }
}

