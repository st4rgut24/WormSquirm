using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TunnelManager : Singleton<TunnelManager>
{
    public Dictionary<Transform, GameObject> EndCapDict; // <GameObject Transform, Enclosing Segment GameObject>
    public Dictionary<Transform, GameObject> TransformCreatedTunnelDict; // <GameObject Transform, Last Created Segment GameObject>

    // The intersection relationship is not bidirectional.
    // The key should be the tunnel that initiates the intersection.
    // For ex. Tunnel A intersects Tunnel B when Tunnel A is created.
    // The new mapping will look like <Tunnel A, [Tunnel B]>
    public Dictionary<GameObject, List<GameObject>> IntersectedTunnelDict;

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
        TransformCreatedTunnelDict = new Dictionary<Transform, GameObject>();
        IntersectedTunnelDict = new Dictionary<GameObject, List<GameObject>>();
        tunnelDisabler = new Disabler(5);
    }

	public GameObject GetGameObjectTunnel(Transform transform)
	{
		if (TransformCreatedTunnelDict.ContainsKey(transform))
		{
            return TransformCreatedTunnelDict[transform];
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

	void OnAddCreatedTunnel(Transform transform, SegmentGo segment, GameObject prevTunnel)
	{
        GameObject endCap = segment.cap;
		AddTunnel(transform, segment, prevTunnel, new List<GameObject>());
		ReplaceEndCap(transform, endCap);
	}

	void OnAddIntersectedTunnel(Transform transform, SegmentGo segment, GameObject prevTunnel, List<GameObject> intersectedTunnels)
	{
        GameObject endCap = segment.cap;

        MapIntersectingTunnels(segment.getTunnel(), intersectedTunnels);

        // exclude the previous tunnel from the list of intersected tunnels if it exists
        List<GameObject> nextTunnels = new List<GameObject>(intersectedTunnels);
        if (intersectedTunnels.Contains(prevTunnel))
        {
            nextTunnels.Remove(prevTunnel);
        }

        AddTunnel(transform, segment, prevTunnel, nextTunnels);
        segment.DestroyCap();
    }

    public bool IsIntersectingInitiator(GameObject initiatorTunnel, GameObject otherTunnel)
    {
        if (IntersectedTunnelDict.ContainsKey(initiatorTunnel))
        {
            return IntersectedTunnelDict[initiatorTunnel].Contains(otherTunnel);
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Check if two tunnels intersect
    /// </summary>
    public bool IsIntersectingTunnel(GameObject tunnel, GameObject otherTunnel)
    {
        return IsIntersectingInitiator(tunnel, otherTunnel) || IsIntersectingInitiator(otherTunnel, tunnel);
    }

    /// <summary>
    /// Update mapping of tunnels to the tunnels they intersect
    /// </summary>
    /// <param name="tunnel">the tunnel</param>
    /// <param name="intersectedTunnels">the tunnels that 'tunnel' intersect</param>
    void MapIntersectingTunnels(GameObject tunnel, List<GameObject> intersectedTunnels)
    {
        if (IntersectedTunnelDict.ContainsKey(tunnel)) // map the current tunnel to the intersected tunnels
        {
            IntersectedTunnelDict[tunnel].AddRange(intersectedTunnels);
        }
        else
        {
            IntersectedTunnelDict[tunnel] = intersectedTunnels;
        }

        //intersectedTunnels.ForEach((intersectedTunnel) => // map each of the intersected tunnels back to the current tunnel
        //{
        //    if (!IntersectedTunnelDict.ContainsKey(intersectedTunnel))
        //    {
        //        IntersectedTunnelDict[intersectedTunnel] = new List<GameObject>();
        //    }

        //    IntersectedTunnelDict[intersectedTunnel].Add(tunnel);
        //});
    }

    void AddTunnel(Transform transform, SegmentGo segmentGo, GameObject prevTunnel, List<GameObject> nextTunnels)
	{
        Corridor corridor = segmentGo.corridor;
        GameObject tunnel = segmentGo.getTunnel();

        if (!TransformCreatedTunnelDict.ContainsKey(transform))
        {
            TransformCreatedTunnelDict[transform] = tunnel;
        }

        Segment segment = SegmentManager.Instance.AddTunnelSegment(tunnel, prevTunnel, nextTunnels, corridor.ring, corridor.prevRing);
        AgentManager.Instance.InitTransformSegmentDict(transform, segment);
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
            TransformCreatedTunnelDict[playerTransform] = UpdatedSegment.tunnel;
            Debug.Log("Player has not moved to a new segment " + UpdatedSegment.tunnel.name);
        }
        else
        {
            Debug.Log("Player has not moved to a new segment. Stuck in " + TransformCreatedTunnelDict[playerTransform]?.name);
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

