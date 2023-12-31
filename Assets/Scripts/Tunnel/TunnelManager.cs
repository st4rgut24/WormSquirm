using UnityEngine;
using System.Collections;

public class TunnelManager : Singleton<TunnelManager>
{

    TunnelInsiderManager tunnelInsiderManager;
    TunnelCreatorManager tunnelCreatorManager;
	TunnelIntersectorManager tunnelIntersectorManager;
	TunnelActionManager tunnelActionManager;

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
}

