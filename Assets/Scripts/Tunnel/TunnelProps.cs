public class TunnelProps
{
    // Properties with getters and setters
    public int TunnelSegments { get; set; }
    public float SegmentSpacing { get; set; }
    public float TunnelRadius { get; set; }
    public float NoiseScale { get; set; }

    // Constructor to initialize properties
    public TunnelProps(int tunnelSegments, float segmentSpacing, float tunnelRadius, float noiseScale)
    {
        TunnelSegments = tunnelSegments; // the number of sides to a tunnel
        SegmentSpacing = segmentSpacing;
        TunnelRadius = tunnelRadius;
        NoiseScale = noiseScale; // variation in radius of a tunnel
    }
}
