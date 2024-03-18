using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class PlayerManager: Singleton<PlayerManager>
{
	public static event Action<GameObject> SpawnMainPlayerEvent;

	public GameObject MainPlayerGo;
	public GameObject PlayerGo;

	public GameObject MainPlayerInst;
	public MainPlayer mainPlayer;

	public List<GameObject> Players;

	public const float PlayerHealth = 100;

    protected void Awake()
	{
		Players = new List<GameObject>();
    }

	public void InitPlayer(Vector3 location, Segment initSegment)
	{
        MainPlayerInst = Spawn(MainPlayerGo, location);
        mainPlayer = MainPlayerInst.GetComponent<MainPlayer>();
		mainPlayer.curSegment = initSegment;
		AgentManager.Instance.SetTransformSegmentDict(mainPlayer.transform, initSegment);

		SpawnMainPlayerEvent?.Invoke(MainPlayerInst);
    }

	public GameObject GetMainPlayer()
	{
		return MainPlayerInst;
	}

    GameObject Spawn(GameObject Player, Vector3 location)
	{
		GameObject player = AgentManager.Instance.CreateAgent(Player);
		player.transform.position = location;
		Players.Add(player);

		return player;
	}

	public bool hasPlayers()
	{
		return this.Players.Count > 0;
	}

    private void OnDisable()
    {
        
    }
}

