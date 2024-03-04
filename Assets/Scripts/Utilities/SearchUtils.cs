using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SearchUtils
{
    /// <summary>
    /// Find a list of segments that connects start segment to final segment
    /// </summary>
    /// <param name="finalSegment">the last segment</param>
    /// <param name="startSegment">the first segment</param>
    /// <returns>list of segments connecting start segment to final segment</returns>
    public static List<Segment> dfsConnectSegments(Segment finalSegment, Segment startSegment)
    {
        // Debug.Log("Get path from start tunnel " + startSegment.tunnel.name + " to end tunnel " + finalSegment.tunnel.name);

        List<Segment> path = new List<Segment>();
        HashSet<GameObject> visited = new HashSet<GameObject>();

        // Perform DFS starting from the final segment
        bool pathFound = DfsHelper(startSegment, finalSegment, path, visited);

        if (!pathFound)
        {
            throw new Exception("No path found from start segment " + startSegment.tunnel.name + " to end segment " + finalSegment.tunnel.name);
        }

        return path;
    }

    private static bool DfsHelper(Segment currentSegment, Segment finalSegment, List<Segment> path, HashSet<GameObject> visited)
    {
        // Add the current segment to the path
        path.Add(currentSegment);

        // Mark the current segment as visited
        visited.Add(currentSegment.tunnel);

        // Check if we have reached the start segment
        if (currentSegment == finalSegment)
        {
            return true;
        }

        // Explore the previous segments
        foreach (GameObject prevTunnel in currentSegment.getNextTunnels())
        {
            if (prevTunnel.activeSelf && !visited.Contains(prevTunnel)) // search tunnels that are active and not yet visited
            {
                Segment nextSegment = SegmentManager.Instance.GetSegmentFromObject(prevTunnel);
                // Recursively explore the previous segments
                if (DfsHelper(nextSegment, finalSegment, path, visited))
                {
                    return true;
                }
            }
        }

        // If no path is found from this segment, backtrack and remove it from the path
        path.Remove(currentSegment);

        return false;
    }

/// <summary>
/// Use breadth first search to find all segment swithin n segments from curSegment
/// </summary>
/// <param name="startSegment">segment to start searching from</param>
/// <param name="n">number of segments to search for from cur segment</param>
/// <returns>list of tunnels within search radius</returns>
public static List<GameObject> bfsSegments(Segment startSegment, int n)
	{
        List<GameObject> adjacentSegments = new List<GameObject>() { startSegment.tunnel }; // current segment is proximal to itself

        Queue<Segment> queue = new Queue<Segment>();
        HashSet<Segment> visited = new HashSet<Segment>();

        queue.Enqueue(startSegment);
        visited.Add(startSegment);

        while (queue.Count > 0 && n > 0)
        {
            int levelSize = queue.Count;

            for (int i = 0; i < levelSize; i++)
            {
                Segment currentSegment = queue.Dequeue();

                // Find next segments
                foreach (var nextSegmentGo in currentSegment.getNextTunnels())
                {
                    RecordSegment(nextSegmentGo, visited, queue, adjacentSegments);
                }
            }

            n--;
        }

        return adjacentSegments;
    }

    /// <summary>
    /// Helper method for search
    /// </summary>
    /// <param name="segmentGo">the current segment gameobject</param>
    /// <param name="visited">set of visited segments</param>
    /// <param name="queue">queue of segments to continue seraching from</param>
    /// <param name="adjacentSegments">segment gameobjects that are within radius</param>
    private static void RecordSegment(GameObject segmentGo, HashSet<Segment> visited, Queue<Segment> queue, List<GameObject> adjacentSegments)
    {
        Segment segment = SegmentManager.Instance.GetSegmentFromObject(segmentGo);

        if (segment != null && !visited.Contains(segment))
        {
            queue.Enqueue(segment);
            visited.Add(segment);
            adjacentSegments.Add(segment.tunnel);
        }
    }
}

