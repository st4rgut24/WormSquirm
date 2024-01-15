using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SearchUtils
{
	/// <summary>
	/// Use breadth first search to find all segment swithin n segments from curSegment
	/// </summary>
	/// <param name="startSegment">segment to start searching from</param>
	/// <param name="n">number of segments to search for from cur segment</param>
	/// <returns>list of tunnels within search radius</returns>
	public static List<GameObject> bfsSegments(Segment startSegment, int n)
	{
        List<GameObject> adjacentSegments = new List<GameObject>();

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

                // Find previous segments
                foreach (var prevSegmentGo in currentSegment.getPrevTunnels())
                {
                    RecordSegment(prevSegmentGo, visited, queue, adjacentSegments);
                }

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

