using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>RingFactory</c> creates the rings composing a Tunnel Segment
/// Each semgent has two rings resulting in a cylinder
/// </summary>
public class RingFactory
{
    /**
     * <param name="center">The center of the ring</param>
     * <param name="normal">The vector perpendicular to the ring</param>
     */
    public static Ring get(float radius, float vertexSpacing, int vertexCount, Vector3 normal, Vector3 center)
    {
        return new Ring(radius, vertexCount, vertexSpacing, normal, center);
    }
}

