using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TunnelManager : Singleton<TunnelManager>
{
    public Dictionary<Transform, GameObject> TransformSegmentDict; // <GameObject Transform, Enclosing Segment GameObject>
    public Dictionary<string, Segment> SegmentDict; // <tunnel name, Segment)

	public TunnelProps defaultProps;

    TunnelInsiderManager tunnelInsiderManager;
    TunnelCreatorManager tunnelCreatorManager;
	TunnelIntersectorManager tunnelIntersectorManager;
	TunnelActionManager tunnelActionManager;

    Disabler tunnelDisabler;

    //todo: use separation value to calculate the tunnel segment's transform
    float separation = 1f; // separation between the player and the end of the active tunnel

	const int tunnelSegments = 7;
    const float segmentSpacing = 1.33f;
    const float tunnelRadius = 5.13f;
    const float noiseScale = .8f;

    private void OnEnable()
    {
        TunnelCreatorManager.OnAddCreatedTunnel += OnAddCreatedTunnel;
        TunnelIntersectorManager.OnAddIntersectedTunnel += OnAddCreatedTunnel;

		Agent.OnMove += tunnelDisabler.Disable;
    }

    private void Awake()
    {
        defaultProps = new TunnelProps(tunnelSegments, segmentSpacing, tunnelRadius, noiseScale);
		SegmentDict = new Dictionary<string, Segment>();

		TransformSegmentDict = new Dictionary<Transform, GameObject>();
		tunnelDisabler = new Disabler(GameManager.Instance.GetGrid(GridType.Tunnel), 3);
    }

	public void AddGameObjectSegment(Transform transform, GameObject segmentGo)
	{
		TransformSegmentDict[transform] = segmentGo;
	}

	/// <summary>
	/// Is the transform inside a tunnel
	/// </summary>
	/// <param name="transform">The object's transform</param>
	/// <returns></returns>
	public bool isInTunnel(Transform transform)
	{
		return TransformSegmentDict.ContainsKey(transform);
    }

	public GameObject GetGameObjectSegment(Transform transform)
	{
		if (TransformSegmentDict.ContainsKey(transform))
		{
            return TransformSegmentDict[transform];
        }
		else
		{
			return null;
		}
	}

	void OnAddCreatedTunnel(GameObject tunnel, GameObject prevTunnel, List<GameObject> nextTunnels)
	{
		Segment segment = new Segment(tunnel, prevTunnel);

		if (prevTunnel != null)
		{
            Segment prevSegment = SegmentDict[prevTunnel.name];
            prevSegment.setNextTunnel(tunnel);
        }

		segment.setNextTunnels(nextTunnels);
 		SegmentDict.Add(tunnel.name, segment);
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

    private void OnDisable()
    {
        TunnelCreatorManager.OnAddCreatedTunnel -= OnAddCreatedTunnel;
        TunnelIntersectorManager.OnAddIntersectedTunnel -= OnAddCreatedTunnel;

        Agent.OnMove -= tunnelDisabler.Disable;
    }
}

