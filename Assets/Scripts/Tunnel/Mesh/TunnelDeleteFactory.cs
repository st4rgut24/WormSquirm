using System;
using System.Collections.Generic;
using UnityEngine;

public class TunnelDeleteFactory
{
	public TunnelDeleteFactory()
	{
	}

	public static TunnelDelete Get(GameObject tunnel, List<Ray> rays, bool isExtend)
	{
		if (isExtend)
		{
            return new TunnelDelete(tunnel, rays);
		}
		else
		{
            return new InvertedTunnelDelete(tunnel, rays);
        }
    }
}

