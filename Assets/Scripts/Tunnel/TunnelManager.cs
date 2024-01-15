using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TunnelManager : Singleton<TunnelManager>
{
    public Dictionary<Transform, GameObject> EndCapDict; // <GameObject Transform, Enclosing Segment GameObject>
    public Dictionary<Transform, GameObject> TransformTunnelDict; // <GameObject Transform, Enclosing Segment GameObject>

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

		EndCapDict = new Dictionary<Transform, GameObject>();
        TransformTunnelDict = new Dictionary<Transform, GameObject>();
		tunnelDisabler = new Disabler(5);
    }

	public void AddGameObjectSegment(Transform transform, GameObject segmentGo)
	{
		TransformTunnelDict[transform] = segmentGo;
	}

	/// <summary>
	/// Is the transform inside a tunnel
	/// </summary>
	/// <param name="transform">The object's transform</param>
	/// <returns></returns>
	public bool isInTunnel(Transform transform)
	{
		return TransformTunnelDict.ContainsKey(transform);
    }

	public GameObject GetGameObjectTunnel(Transform transform)
	{
		if (TransformTunnelDict.ContainsKey(transform))
		{
            return TransformTunnelDict[transform];
        }
		else
		{
			return null;
		}
	}

	void OnAddCreatedTunnel(Transform transform, SegmentGo segmentGo, GameObject prevTunnel, List<GameObject> nextTunnels)
	{
		GameObject endCap = segmentGo.cap;
		GameObject tunnel = segmentGo.segment;

        SegmentManager.Instance.AddTunnelSegment(tunnel, prevTunnel, nextTunnels);

		ReplaceEndCap(transform, endCap);
	}

	void ReplaceEndCap(Transform transform, GameObject endCap)
	{
		if (EndCapDict.ContainsKey(transform))
		{
			GameObject prevCap = EndCapDict[transform];

			if(prevCap != null)
			{
                Destroy(prevCap);
            }
        }

		EndCapDict[transform] = endCap;
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

