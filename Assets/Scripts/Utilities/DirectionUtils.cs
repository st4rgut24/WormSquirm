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
    public static Heading GetHeading(Transform transform, int offsetMultiplier)
    {
        Vector3 position = transform.position;
        Vector3 offsetPos = position + transform.forward * offsetMultiplier;
        return new Heading(offsetPos, transform.forward);
    }
}

