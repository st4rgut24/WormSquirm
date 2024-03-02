using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements.Experimental;

public class TunnelManager : Singleton<TunnelManager>
{
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

    Grid tunnelGrid;

    Disabler tunnelDisabler;

    public const int minSegmentLength = 5; // the maximum length of a tunnel segment

    //todo: use separation value to calculate the tunnel segment's transform
    float separation = 1f; // separation between the player and the end of the active tunnel

    public const float tunnelRadius = 5.13f;

	const int tunnelSides = 8;
    const float segmentSpacing = 1.33f;
    // issue with noiseScale != 0: dist from center line to ring edge should be constant to
    // avoid any error when calculating tunnel edges
    const float noiseScale = 0;

    private void OnEnable()
    {
        TunnelCreatorManager.OnAddCreatedTunnel += OnAddCreatedTunnel;
        TunnelIntersectorManager.OnAddIntersectedTunnelSuccess += OnAddIntersectedTunnel;

		Agent.OnDig += tunnelDisabler.Disable;
    }

    private void Awake()
    {
        defaultProps = new TunnelProps(tunnelSides, segmentSpacing, tunnelRadius, noiseScale);

		//EndCapDict = new Dictionary<Transform, GameObject>();
        IntersectedTunnelDict = new Dictionary<GameObject, List<GameObject>>();
        tunnelDisabler = new Disabler(5);
    }

    /// <summary>
    /// Get tunnels near to player
    /// </summary>
    /// <param name="playerTransform">the player</param>
    /// <returns></returns>
    /// <exception cref="Exception">if no tunnels found near player</exception>
    public List<GameObject> GetProximalTunnels(Transform playerTransform)
    {
        if (tunnelDisabler.ProximalObjectDict.ContainsKey(playerTransform))
        {
            return tunnelDisabler.ProximalObjectDict[playerTransform];
        }
        else
        {
            throw new Exception("No tunnels corresponding to player transform " + playerTransform);
        }
    }

	public GameObject GetGameObjectTunnel(Transform playerTransform)
	{
        Segment segment = AgentManager.Instance.GetSegment(playerTransform);

		if (segment != null)
		{
            return segment.tunnel;
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

	void OnAddCreatedTunnel(Transform playerTransform, SegmentGo segment, GameObject prevTunnel)
	{
        GameObject endCap = segment.GetEndCap();
        List<GameObject> neighborTunnels = InitTunnelList(prevTunnel);
		AddTunnel(playerTransform, segment, neighborTunnels, prevTunnel);
		//ReplaceEndCap(playerTransform, endCap);        
	}

    /// <summary>
    /// Add intersecting tunnels info
    /// </summary>
    /// <param name="playerTransform">player pos</param>
    /// <param name="segment">new tunnel segment that intersects others</param>
    /// <param name="prevTunnel">tunnel from which the player dug the intersected segment</param>
    /// <param name="intersectedTunnels">a list of intersected tunnel segments</param>
	void OnAddIntersectedTunnel(Transform playerTransform, SegmentGo segment, GameObject prevTunnel, List<GameObject> intersectedTunnels)
	{
        GameObject endCap = segment.GetEndCap();

        MapIntersectingTunnels(segment.getTunnel(), intersectedTunnels);

        // exclude the previous tunnel from the list of intersected tunnels if it exists
        List<GameObject> nextTunnels = new List<GameObject>(intersectedTunnels);
        if (intersectedTunnels.Contains(prevTunnel))
        {
            nextTunnels.Remove(prevTunnel);
        }

        List<GameObject> connectingTunnels = InitTunnelList(prevTunnel);
        connectingTunnels.AddRange(nextTunnels);

        AddTunnel(playerTransform, segment, connectingTunnels, prevTunnel);

        Debug.Log("There are " + connectingTunnels.Count + " intersecting tunnels");

        if (connectingTunnels.Count > 1 || prevTunnel == null) // the new segment intersects the previous and another tunnel in front, creating a corridor (no need for cap)
        {
            segment.IntersectEndCap();
        }
    }

    public List<GameObject> InitTunnelList(GameObject InitTunnel)
    {
        List<GameObject> tunnelList = new List<GameObject>();

        if (InitTunnel != null)
        {
            tunnelList.Add(InitTunnel);
        }

        return tunnelList;
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
    }

    void AddTunnel(Transform playerTransform, SegmentGo segmentGo, List<GameObject> nextTunnels, GameObject prevTunnel)
	{

        Cap startCap = segmentGo.StartCap;
        Cap endCap = segmentGo.EndCap;
        GameObject tunnel = segmentGo.getTunnel();

        Segment segment = SegmentManager.Instance.AddTunnelSegment(segmentGo, nextTunnels, endCap.ring, startCap.ring);
        AgentManager.Instance.InitTransformSegmentDict(playerTransform, segment);

        Debug.Log("Add segment to grid at position " + segment.getCenter());
        tunnelGrid.AddGameObject(segment.getCenter(), segmentGo.getTunnel());

        if (prevTunnel != null)
        {
            RemovePrevTunnelCap(prevTunnel);
        }
    }

    void RemovePrevTunnelCap(GameObject prevTunnel)
    {
        Segment prevSegment = SegmentManager.Instance.GetSegmentFromObject(prevTunnel);

        if (prevSegment.HasDeadEndCap())
        {
            SegmentGo segmentGo = prevSegment.segmentGo;
            segmentGo.DestroyEndCap();
        }
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
            Debug.Log("Player has moved to a new segment " + UpdatedSegment.tunnel.name);
        }
        //else
        //{
        //    Debug.Log("Player has not moved to a new segment. Stuck in " + TransformCreatedTunnelDict[playerTransform]?.name);
        //}
    }

	//void ReplaceEndCap(Transform transform, GameObject endCap)
	//{
	//	if (EndCapDict.ContainsKey(transform))
	//	{
	//		GameObject prevCap = EndCapDict[transform];

	//		if(prevCap != null)
	//		{
 //               Destroy(prevCap);
 //           }
 //       }

	//	EndCapDict[transform] = endCap;
	//}

    // Use this for initialization
    void Start()
	{
		tunnelInsiderManager = TunnelInsiderManager.Instance;
		tunnelCreatorManager = TunnelCreatorManager.Instance;
		tunnelActionManager = TunnelActionManager.Instance;
		tunnelIntersectorManager = TunnelIntersectorManager.Instance;

        tunnelGrid = GameManager.Instance.GetGrid(GridType.Tunnel);
    }

    // Update is called once per frame
    void Update()
	{
			
	}

    private void OnDisable()
    {
        TunnelCreatorManager.OnAddCreatedTunnel -= OnAddCreatedTunnel;
        TunnelIntersectorManager.OnAddIntersectedTunnelSuccess -= OnAddIntersectedTunnel;

        Agent.OnDig -= tunnelDisabler.Disable;
    }
}

