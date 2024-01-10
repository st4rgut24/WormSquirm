using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : Singleton<PlayerManager>
{
	public GameObject PlayerGO;
    public List<GameObject> Players;

    Vector3 defaultSpawnLoc = new Vector3(12, 10, 7);

    private void Awake()
    {
		Players = new List<GameObject>();
    }

    // Use this for initialization
    void Start()
	{
		Spawn();
	}

	void Spawn()
	{
		GameObject player = Instantiate(PlayerGO);
		player.transform.position = defaultSpawnLoc;
		Players.Add(player);
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

