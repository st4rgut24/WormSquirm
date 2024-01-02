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

    Dictionary<Transform, GameObject> PrevCreatedSegmentDict; // <Player Transform, the last tunnel segment player created>
    Dictionary<Transform, Action> LastTunnelActionDict; // <Player Transform, the last tunnel action>

    public static event Action<Transform, GameObject, List<GameObject>, Action> OnIntersectTunnel; // intersect an existing tunnel
    public static event Action<Transform, Dictionary<Transform, GameObject>> OnCreateTunnel; // create a new unobstructed tunnel
    public static event Action<Transform> OnFollowTunnel; // follow path of existing tunnel

    public enum Action {
        Follow,
        Intersect,
        Create,
        None
    }

    private void OnEnable()
    {
        Agent.OnMove += TunnelAction;
    }

    private void Awake()
    {
        PrevCreatedSegmentDict = new Dictionary<Transform, GameObject>();
        LastTunnelActionDict = new Dictionary<Transform, Action>();
    }

    // Start is called before the first frame update
    void Start()
    {
        tunnelGrid = GameManager.Instance.GetGrid(GridType.Tunnel);
    }

    /// <summary>
    /// Take an action based on the location of the next tunnel segment
    /// </summary>
    /// <param name="playerTransform">The projected location of the tunnel/player</param>
    void TunnelAction(Transform playerTransform)
    {
        List<GameObject> otherTunnels = tunnelGrid.GetGameObjects(playerTransform.position);

        GameObject EnclosingTunnel = TunnelUtils.getEnclosingObject(playerTransform.position, otherTunnels);

        Action lastTunnelAction = LastTunnelActionDict.ContainsKey(playerTransform) ? LastTunnelActionDict[playerTransform] : Action.None;

        Debug.Log("Enclosing tunnel is " + EnclosingTunnel?.name);

        if (IsIntersect(EnclosingTunnel, lastTunnelAction)) // intersect
        {
             Debug.Log("Tunnel Action Intersect");
            GameObject prevSegment = PrevCreatedSegmentDict[playerTransform];
            OnIntersectTunnel?.Invoke(playerTransform, prevSegment, otherTunnels, lastTunnelAction);
            LastTunnelActionDict[playerTransform] = Action.Intersect;
        }
        else if (EnclosingTunnel == null)
        {
            Debug.Log("Tunnel Action Create");
            OnCreateTunnel?.Invoke(playerTransform, PrevCreatedSegmentDict);
            LastTunnelActionDict[playerTransform] = Action.Create;
        }
        else
        {
            Debug.Log("Tunnel Action Follow");
            OnFollowTunnel?.Invoke(playerTransform);
            LastTunnelActionDict[playerTransform] = Action.Follow;
        }
    }

    bool IsIntersect(GameObject enclosingTunnel, Action lastTunnelAction)
    {
        return enclosingTunnel == null ? lastTunnelAction == Action.Follow : lastTunnelAction == Action.Create;
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
