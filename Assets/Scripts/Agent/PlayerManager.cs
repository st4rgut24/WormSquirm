using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : Singleton<PlayerManager>
{
	public GameObject MainPlayerGo;
	public GameObject PlayerGo;

	public GameObject MainPlayerInst;

    public List<GameObject> Players;

    Vector3 defaultSpawnLoc = new Vector3(12, 10, 7);

    private void OnEnable()
    {
		SegmentManager.OnNewSegment += OnEnterNewSegment;
    }

    private void Awake()
    {
		Players = new List<GameObject>();
        MainPlayerInst = Spawn(MainPlayerGo);
    }

    public GameObject GetMainPlayer()
	{
		return MainPlayerInst;
	}

    // Use this for initialization
    void Start()
	{
		Ray ray = new Ray(MainPlayerInst.transform.position, MainPlayerInst.transform.forward);
    }

    GameObject Spawn(GameObject Player)
	{
		GameObject player = Instantiate(Player);
		player.transform.position = defaultSpawnLoc;
		Players.Add(player);

		return player;
	}

	/// <summary>
	/// When player enters new tunnel, adjust the player's angle of attack
	/// </summary>
	/// <param name="transform"></param>
	/// <param name="segment"></param>
	public void OnEnterNewSegment(Transform transform, Segment segment)
	{
		Player movedAgent = transform.gameObject.GetComponent<Player>();
        Quaternion tunnelDirection = Quaternion.LookRotation(segment.forward);
		Vector3 upDownRotation = new Vector3(tunnelDirection.eulerAngles.x, 0, 0);
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
    }
}

