using System;
using System.Collections.Generic;
using UnityEngine;

public class TunnelDeleteFactory
{
	public TunnelDeleteFactory()
	{
	}

	public static TunnelDelete Get(GameObject tunnel, List<Ray> rays, bool isInside)
	{
		if (isInside)
		{
			return new InvertedTunnelDelete(tunnel, rays);
		}
		else
		{
			return new TunnelDelete(tunnel, rays);
		}
	}
}

