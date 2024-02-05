using System;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

public class DirectionUtils
{
    const float RightAngle = 90;

    public static Vector3 GetNormalizedDirection(Vector3 startPos, Vector3 endPos)
    {
        Vector3 translation = endPos - startPos;
        return Vector3.Normalize(translation);
    }

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

    /// <summary>
    /// Determine if two transforms can be considered to face the same direction
    /// </summary>
    /// <param name="transformForward">player's forward direction</param>
    /// <param name="segmentForward">segment's forward direction</param>
    public static bool isDirectionsAligned(Vector3 transformForward, Vector3 segmentForward)
    {
        float angle = Vector3.Angle(transformForward, segmentForward);
        return Math.Abs(angle) < RightAngle;
    }

    /// <summary>
    /// Determine the rotation of player based off of direction traveling through a segment
    /// </summary>
    /// <param name="transformForward">player's forward direction</param>
    /// <param name="segmentForward">segment's forward direction</param>
    /// <returns>rotation along the x axis (up,down) from player perspsective</returns>
    public static float GetUpDownRotation(Vector3 transformForward, Vector3 segmentForward)
    {
        Vector3 direction = isDirectionsAligned(transformForward, segmentForward) ? segmentForward : -segmentForward; // go opposite direction of segment if facing in that general direction
        //Debug.Log("Angle between player and segment forward is " + angle + " Direction in new segment is " + direction);
        Quaternion curRotation = Quaternion.LookRotation(direction);
        Quaternion rotation = Quaternion.LookRotation(direction);
        //return new Vector3(rotation.eulerAngles.x, 0, 0);
        return rotation.eulerAngles.x;
    }
}

