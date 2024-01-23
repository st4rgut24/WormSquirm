using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : AgentManager
{
	public GameObject MainPlayerGo;
	public GameObject PlayerGo;

	public GameObject MainPlayerInst;

    public List<GameObject> Players;

    Vector3 defaultSpawnLoc = new Vector3(26, 10, 7);

    private void OnEnable()
    {
        SegmentManager.OnNewSegment += OnEnterNewSegment;
        TunnelIntersectorManager.OnAddIntersectedTunnel += OnEnterIntersectedTunnel;
    }

    protected override void Awake()
    {
		base.Awake();
		Players = new List<GameObject>();
    }

    public GameObject GetMainPlayer()
	{
		return MainPlayerInst;
	}

    // Use this for initialization
    void Start()
	{
        MainPlayerInst = Spawn(MainPlayerGo);

        Ray ray = new Ray(MainPlayerInst.transform.position, MainPlayerInst.transform.forward);
    }

    GameObject Spawn(GameObject Player)
	{
		GameObject player = CreateAgent(Player);
		player.transform.position = defaultSpawnLoc;
		Players.Add(player);

		return player;
	}

	public void OnEnterIntersectedTunnel(Transform transform, SegmentGo segmentGo, GameObject prevTunnel, List<GameObject> nextTunnels)
	{
    }

    /// <summary>
    /// When player enters new tunnel, adjust the player's angle of attack
    /// </summary>
    /// <param name="transform">player transform</param>
    /// <param name="segment">segment of tunnel player is in</param>
    /// // TODO: consider moving this callback to a new classs called AgentManager, since it should affect both bots and players
    public void OnEnterNewSegment(Transform transform, Segment segment)
	{
		Player movedAgent = transform.gameObject.GetComponent<Player>();
		movedAgent.UpdateSegment(segment);
        Vector3 upDownRotation = DirectionUtils.GetUpDownRotation(transform.forward, movedAgent.curSegmentForward);
        movedAgent.ChangeRotation(upDownRotation, true);
	}

	public bool hasPlayers()
	{
		return this.Players.Count > 0;
	}

	// Update is called once per frame
	void Update()
	{
			
	}

    private void OnDisable()
    {
        SegmentManager.OnNewSegment -= OnEnterNewSegment;
        TunnelIntersectorManager.OnAddIntersectedTunnel += OnEnterIntersectedTunnel;
    }
}

