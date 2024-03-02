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

    public const float PlayerHealth = 100;

    Vector3 defaultSpawnLoc = new Vector3(26, 10, 7);

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


	public bool hasPlayers()
	{
		return this.Players.Count > 0;
	}

	// Update is called once per frame
	void Update()
	{
			
	}
}

