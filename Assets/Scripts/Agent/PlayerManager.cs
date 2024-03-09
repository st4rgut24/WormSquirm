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

	Vector3 defaultSpawnLoc = new Vector3(26, 10, 7);

    protected void OnEnable()
    {
		BotManager.Instance.DestroyBotEvent += OnRemoveBot;
    }

    protected void Awake()
	{
		Players = new List<GameObject>();

        MainPlayerInst = Spawn(MainPlayerGo);
        mainPlayer = MainPlayerInst.GetComponent<MainPlayer>();
    }

	public GameObject GetMainPlayer()
	{
		return MainPlayerInst;
	}

	public void OnRemoveBot(GameObject HitObject)
	{
		mainPlayer.RemoveCollidedObject(HitObject);
	}

    // Use this for initialization
    void Start()
	{
		SpawnMainPlayerEvent?.Invoke(mainPlayer.gameObject);

        Ray ray = new Ray(MainPlayerInst.transform.position, MainPlayerInst.transform.forward);
    }

    GameObject Spawn(GameObject Player)
	{
		GameObject player = AgentManager.Instance.CreateAgent(Player);
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

    protected void OnDisable()
    {
        BotManager.Instance.DestroyBotEvent += OnRemoveBot;
    }
}

