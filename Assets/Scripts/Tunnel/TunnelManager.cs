using UnityEngine;
using System.Collections;

public class TunnelManager : Singleton<TunnelManager>
{
	public TunnelProps defaultProps;

    TunnelInsiderManager tunnelInsiderManager;
    TunnelCreatorManager tunnelCreatorManager;
	TunnelIntersectorManager tunnelIntersectorManager;
	TunnelActionManager tunnelActionManager;

	//todo: use separation value to calculate the tunnel segment's transform
	float separation = 1f; // separation between the player and the end of the active tunnel

	const int tunnelSegments = 7;
    const float segmentSpacing = 1.33f;
    const float tunnelRadius = 5.13f;
    const float noiseScale = .8f;

    private void Awake()
    {
        defaultProps = new TunnelProps(tunnelSegments, segmentSpacing, tunnelRadius, noiseScale);
    }

    // Use this for initialization
    void Start()
	{
		tunnelInsiderManager = TunnelInsiderManager.Instance;
		tunnelCreatorManager = TunnelCreatorManager.Instance;
		tunnelActionManager = TunnelActionManager.Instance;
		tunnelIntersectorManager = TunnelIntersectorManager.Instance;
	}

	// Update is called once per frame
	void Update()
	{
			
	}
}

