using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : Singleton<PlayerManager>
{
	public GameObject MainPlayerGo;
	public GameObject PlayerGo;

    public List<GameObject> Players;

    Vector3 defaultSpawnLoc = new Vector3(12, 10, 7);

    private void Awake()
    {
		Players = new List<GameObject>();
    }

    // Use this for initialization
    void Start()
	{
		GameObject PlayerGo = Spawn();
		Ray ray = new Ray(PlayerGo.transform.position, PlayerGo.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 50.0f);
    }

    GameObject Spawn()
	{
		GameObject player = Instantiate(MainPlayerGo);
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

