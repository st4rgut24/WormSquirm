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

    public static event Action<Transform, GameObject, Heading, List<GameObject>, Action> OnIntersectTunnel; // intersect an existing tunnel
    public static event Action<Transform, Action> OnCreateTunnel; // create a new unobstructed tunnel
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
        List<GameObject> otherTunnels = tunnelGrid.GetGameObjects(playerTransform.position, 1);

        GameObject EnclosingTunnel = TunnelUtils.getEnclosingObject(playerTransform.position, otherTunnels);

        Action lastTunnelAction = LastTunnelActionDict.ContainsKey(playerTransform) ? LastTunnelActionDict[playerTransform] : Action.None;

        //Debug.Log("Enclosing tunnel is " + EnclosingTunnel?.name);

        if (IsIntersect(EnclosingTunnel, lastTunnelAction)) // intersect
        {
            Debug.Log("TunnelAction Intersect");
            GameObject prevSegment = TunnelManager.Instance.GetGameObjectSegment(playerTransform);
            //GameObject prevSegment = PrevCreatedSegmentDict[playerTransform];
            Heading heading = PrevHeadingDict[playerTransform];
            OnIntersectTunnel?.Invoke(playerTransform, prevSegment, heading, otherTunnels, lastTunnelAction);
            LastTunnelActionDict[playerTransform] = Action.Intersect;
        }
        else if (EnclosingTunnel == null)
        {
            Debug.Log("Tunnel Action Create");
            OnCreateTunnel?.Invoke(playerTransform, lastTunnelAction);
            LastTunnelActionDict[playerTransform] = Action.Create;
        }
        else
        {
            Debug.Log("Tunnel Action Follow");
            OnFollowTunnel?.Invoke(playerTransform);
            LastTunnelActionDict[playerTransform] = Action.Follow;
        }

        PrevHeadingDict[playerTransform] = new Heading(playerTransform);
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
