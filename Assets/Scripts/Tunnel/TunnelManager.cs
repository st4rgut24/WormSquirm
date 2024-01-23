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

    public const int minSegmentLength = 5; // the maximum length of a tunnel segment

    //todo: use separation value to calculate the tunnel segment's transform
    float separation = 1f; // separation between the player and the end of the active tunnel

    public const float tunnelRadius = 5.13f;

	const int tunnelSegments = 7;
    const float segmentSpacing = 1.33f;
    const float noiseScale = .8f;

    private void OnEnable()
    {
        TunnelCreatorManager.OnAddCreatedTunnel += OnAddCreatedTunnel;
        TunnelIntersectorManager.OnAddIntersectedTunnel += OnAddIntersectedTunnel;

		Agent.OnDig += tunnelDisabler.Disable;
    }

    private void Awake()
    {
        defaultProps = new TunnelProps(tunnelSegments, segmentSpacing, tunnelRadius, noiseScale);

		EndCapDict = new Dictionary<Transform, GameObject>();
        TransformTunnelDict = new Dictionary<Transform, GameObject>();
		tunnelDisabler = new Disabler(5);
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

	public bool ExceedsMinSegmentLength(float segmentLength)
	{
		Debug.Log("Segment has length " + segmentLength);
		return segmentLength >= minSegmentLength;
	}

	void OnAddCreatedTunnel(Transform transform, SegmentGo segment, GameObject prevTunnel, List<GameObject> nextTunnels)
	{
        GameObject endCap = segment.cap;
		AddTunnel(transform, segment, prevTunnel, nextTunnels);
		ReplaceEndCap(transform, endCap);
	}

	void OnAddIntersectedTunnel(Transform transform, SegmentGo segment, GameObject prevTunnel, List<GameObject> nextTunnels)
	{
        GameObject endCap = segment.cap;
        AddTunnel(transform, segment, prevTunnel, nextTunnels);
        segment.DestroyCap();
    }

    void AddTunnel(Transform transform, SegmentGo segment, GameObject prevTunnel, List<GameObject> nextTunnels)
	{
        Corridor corridor = segment.corridor;
        GameObject tunnel = segment.getTunnel();

        if (!TransformTunnelDict.ContainsKey(transform))
        {
            TransformTunnelDict[transform] = tunnel;
        }

        SegmentManager.Instance.AddTunnelSegment(tunnel, prevTunnel, nextTunnels, corridor.ring, corridor.prevRing);
    }

    /// <summary>
    /// Update the mapping of players to tunnel gameobjects
    /// </summary>
    /// <param name="playerTransform"></param>
    public void UpdateTransformDict(Transform playerTransform)
	{
		Vector3 playerPosition = playerTransform.position;

		Segment UpdatedSegment = SegmentManager.Instance.UpdateSegmentFromTransform(playerTransform);

		if (UpdatedSegment != null) // if the player entered a new segment
		{
            TransformTunnelDict[playerTransform] = UpdatedSegment.tunnel;
            Debug.Log("Player has not moved to a new segment " + UpdatedSegment.tunnel.name);
        }
        else
        {
            Debug.Log("Player has not moved to a new segment. Stuck in " + TransformTunnelDict[playerTransform]?.name);
        }
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
        TunnelIntersectorManager.OnAddIntersectedTunnel -= OnAddIntersectedTunnel;

        Agent.OnDig -= tunnelDisabler.Disable;
    }
}

