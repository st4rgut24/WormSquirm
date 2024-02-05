public class    TunnelProps
{
    // Properties with getters and setters
    public int TunnelSides { get; set; }
    public float SegmentSpacing { get; set; }
    public float TunnelRadius { get; set; }
    public float NoiseScale { get; set; }
    public float SideLength { get; }

    // Constructor to initialize properties
    public TunnelProps(int tunnelSides, float segmentSpacing, float tunnelRadius, float noiseScale)
    {
        TunnelSides = tunnelSides; // the number of sides to a tunnel
        SegmentSpacing = segmentSpacing;    
        TunnelRadius = tunnelRadius;
        NoiseScale = noiseScale; // variation in radius of a tunnel
        SideLength = MeshUtils.GetSideLength(tunnelSides, TunnelRadius);
    }
}
