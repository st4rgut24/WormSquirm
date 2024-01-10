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

    Dictionary<Transform, Action> LastTunnelActionDict; // <Player Transform, the last tunnel action>
    Dictionary<Transform, Heading> PrevHeadingDict; // <Player Transform, the player's previous position>

    public static event Action<Transform, GameObject, Heading, Heading, List<GameObject>, Action> OnIntersectTunnel; // intersect an existing tunnel
    public static event Action<Transform, Action, Heading> OnCreateTunnel; // create a new unobstructed tunnel
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
        PrevHeadingDict = new Dictionary<Transform, Heading>();
        LastTunnelActionDict = new Dictionary<Transform, Action>();

        tunnelGrid = GameManager.Instance.GetGrid(GridType.Tunnel);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    /// <summary>
    /// Take an action based on the location of the next tunnel segment
    /// </summary>
    /// <param name="playerTransform">The projected location of the tunnel/player</param>
    void TunnelAction(Transform playerTransform)
    {
        Heading TunnelHeading = DirectionUtils.GetHeading(playerTransform, GameManager.Instance.agentOffset);
        List<GameObject> otherTunnels = tunnelGrid.GetGameObjects(TunnelHeading.position, 1);

        GameObject EnclosingTunnel = TunnelUtils.getEnclosingObject(TunnelHeading.position, otherTunnels);

        Action lastTunnelAction = LastTunnelActionDict.ContainsKey(playerTransform) ? LastTunnelActionDict[playerTransform] : Action.None;

        if (IsIntersect(EnclosingTunnel, lastTunnelAction)) // intersect
        {
            //Debug.Log("TunnelAction Intersect");
            GameObject prevSegment = TunnelManager.Instance.GetGameObjectSegment(playerTransform);
            Heading PrevHeading = PrevHeadingDict[playerTransform];
            OnIntersectTunnel?.Invoke(playerTransform, prevSegment, TunnelHeading, PrevHeading, otherTunnels, lastTunnelAction);
            LastTunnelActionDict[playerTransform] = Action.Intersect;
        }
        else if (EnclosingTunnel == null)
        {
            //Debug.Log("Tunnel Action Create");
            OnCreateTunnel?.Invoke(playerTransform, lastTunnelAction, TunnelHeading);
            LastTunnelActionDict[playerTransform] = Action.Create;
        }
        else
        {
            //Debug.Log("Tunnel Action Follow");
            OnFollowTunnel?.Invoke(playerTransform);
            LastTunnelActionDict[playerTransform] = Action.Follow;
        }

        PrevHeadingDict[playerTransform] = TunnelHeading;
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
