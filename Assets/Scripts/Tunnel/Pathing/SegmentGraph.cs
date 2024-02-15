using System;
using System.Collections.Generic;

public class SegmentGraph
{
    private class NodePair
    {
        public Segment Node1 { get; }
        public Segment Node2 { get; }

        public NodePair(Segment node1, Segment node2)
        {
            Node1 = node1;
            Node2 = node2;
        }

        public override int GetHashCode()
        {
            // Ensure the hash code is the same regardless of the order of nodes
            return Node1.GetHashCode() ^ Node2.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is NodePair other)
            {
                // Check for equality regardless of the order of nodes
                return (Node1.Equals(other.Node1) && Node2.Equals(other.Node2)) ||
                       (Node1.Equals(other.Node2) && Node2.Equals(other.Node1));
            }
            return false;
        }
    }

    // Dictionary to store the graph
    private Dictionary<NodePair, Connector> graph = new Dictionary<NodePair, Connector>();

    /// <summary>
    /// Add an edge between two segments
    /// </summary>
    /// <param name="connectingNode">the node the edge belongs to</param>
    /// <param name="node2">the other node this edge touches</param>
    /// <param name="connectingLine">the line belonging to one node and bordering the other</param>
    /// <param name="isContinuous">Whether the edge is continuous between paths (eg relevant for tunnels that extend each other)</param>
    public void AddEdge(Segment connectingNode, Segment node2, Guideline connectingLine, bool isContinuous)
    {
        var pair = new NodePair(connectingNode, node2);

        if (!graph.ContainsKey(pair))
        {
            graph[pair] = new Connector(connectingNode, node2);
        }

        if (isContinuous)
        {
            graph[pair].SetContinuousPath(connectingNode, node2);
        }
        else
        {
            graph[pair].SetIntersectingPath(connectingNode, node2, connectingLine);
        }
    }

    // Get the shared nodes between two nodes
    public Connector GetConnector(Segment node1, Segment node2)
    {
        var pair = new NodePair(node1, node2);

        return graph[pair];
    }
}
