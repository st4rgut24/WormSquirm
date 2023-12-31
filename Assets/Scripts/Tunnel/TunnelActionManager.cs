using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Notifies other GameObjects of Tunnel Actions
/// </summary>
public class TunnelActionManager: Singleton<TunnelActionManager>
{
    Grid tunnelGrid;

    Dictionary<Transform, bool> isTunnelingDict; // <Player Transform, creating tunnel flag>
    Dictionary<Transform, GameObject> PrevCreatedSegmentDict; // <Player Transform, the last tunnel segment player created>

    public static event Action<Transform, List<GameObject>, GameObject> OnIntersectTunnel; // intersect an existing tunnel
    public static event Action<Transform, Dictionary<Transform, GameObject>> OnCreateTunnel; // create a new unobstructed tunnel
    public static event Action<Transform> OnFollowTunnel; // follow path of existing tunnel

    private void OnEnable()
    {
        Agent.OnMove += TunnelAction;
    }

    private void Awake()
    {
        isTunnelingDict = new Dictionary<Transform, bool>();
        PrevCreatedSegmentDict = new Dictionary<Transform, GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        tunnelGrid = GameManager.Instance.GetGrid(GridType.Tunnel);
    }

    /// <summary>
    /// Get boolean flag indicating if the player is actively extending a new tunnel
    /// </summary>
    /// <param name="playerTransform">Transform identifying a player</param>
    /// <returns>True if extending a tunnel or if the key does not exist</returns>
    bool isTunneling(Transform playerTransform)
    {
        if (!isTunnelingDict.ContainsKey(playerTransform))
        {
            isTunnelingDict[playerTransform] = true;
        }

        return isTunnelingDict[playerTransform];
    }

    /// <summary>
    /// Take an action based on the location of the next tunnel segment
    /// </summary>
    /// <param name="playerTransform">The projected location of the tunnel/player</param>
    void TunnelAction(Transform playerTransform)
    {
        List<GameObject> otherTunnels = tunnelGrid.GetGameObjects(playerTransform.position);
        Debug.Log("other tunnels " + otherTunnels.Count);
        GameObject TestEnclosingTunnel = TunnelUtils.getEnclosingObject(playerTransform.position, otherTunnels);
         if (TestEnclosingTunnel == null)
        {
            Debug.Log("No enclosing object at player position " + playerTransform.position);
        }
        GameObject EnclosingTunnel = TunnelUtils.getEnclosingObject(playerTransform.position, otherTunnels);

        if (EnclosingTunnel == null)
        {
            Debug.Log("Tunnel Action Create");
            OnCreateTunnel?.Invoke(playerTransform, PrevCreatedSegmentDict);
            isTunnelingDict[playerTransform] = true;
        }
        else if (isTunneling(playerTransform))
        {
            Debug.Log("Tunnel Action Intersect");
            GameObject prevSegment = PrevCreatedSegmentDict[playerTransform];
            OnIntersectTunnel?.Invoke(playerTransform, otherTunnels, prevSegment);
            isTunnelingDict[playerTransform] = false;
        }
        else
        {
            Debug.Log("Tunnel Action Follow");
            OnFollowTunnel?.Invoke(playerTransform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        Agent.OnMove -= TunnelAction;
    }
}
