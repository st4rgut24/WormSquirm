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

	//public void AcceleratePlayer()
	//{
	//	if (Players.Count > 0)
	//	{
	//		Players[0].GetComponent<Player>().AccelerateInCurrentDirection();

 //       }
 //   }

	public bool hasPlayers()
	{
		return this.Players.Count > 0;
	}

	// Update is called once per frame
	void Update()
	{
			
	}
}

