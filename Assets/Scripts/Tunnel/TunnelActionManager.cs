using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

/// <summary>
/// Notifies other GameObjects of Tunnel Actions
/// </summary>
public class TunnelActionManager: Singleton<TunnelActionManager>
{
    Grid tunnelGrid;

    Dictionary<Transform, Action> LastTunnelActionDict; // <Player Transform, the last tunnel action>
    Dictionary<Transform, Heading> PrevHeadingDict; // <Player Transform, the player's previous position>

    public static event Action<Transform, GameObject, Heading, Heading, List<GameObject>, bool, Ring> OnIntersectTunnel; // intersect an existing tunnel
    public static event Action<Transform, bool, Heading, Ring> OnCreateTunnel; // create a new unobstructed tunnel
    public static event Action<Transform> OnFollowTunnel; // follow path of existing tunnel

    public enum Action {
        Follow,
        Intersect,
        Create,
        None
    }

    private void OnEnable()
    {
        Agent.OnDig += TunnelAction;
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
    void TunnelAction(Transform playerTransform, Vector3 direction)
    {
        Heading TunnelHeading = DirectionUtils.GetHeading(playerTransform.position, direction, GameManager.Instance.agentOffset);


        bool IsTunnelCreated = CreateTunnel(playerTransform, TunnelHeading);

        if (!IsTunnelCreated)
        {
            // TODO: this case should not exist
            Debug.Log("Tunnel Action Follow");
            OnFollowTunnel?.Invoke(playerTransform);
            LastTunnelActionDict[playerTransform] = Action.Follow;
        }

        PrevHeadingDict[playerTransform] = TunnelHeading;
    }

    /// <summary>
    /// Create Tunnel Segments
    /// </summary>
    /// <param name="playerTransform"></param>
    public bool CreateTunnel(Transform playerTransform, Heading TunnelHeading)
    {
        bool extendsTunnel = SegmentManager.Instance.IsExtendingTunnel(playerTransform);

        List<GameObject> otherTunnels = tunnelGrid.GetGameObjects(TunnelHeading.position, 1);

        GameObject EnclosingTunnel = TunnelUtils.getEnclosingObject(TunnelHeading.position, otherTunnels);

        Ray ray = new Ray(playerTransform.position, TunnelHeading.forward);
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 10);

        // TODO: dont use last tunnel action to determine the current tunnel action anymore.
        // if (!isMovingIntoEndCap && EnclosingTunnel == null) || (isMovingIntoEndCap && EnclosingTunnel != null) 
        Action lastTunnelAction = LastTunnelActionDict.ContainsKey(playerTransform) ? LastTunnelActionDict[playerTransform] : Action.None;


        bool isIntersecting = IsIntersect(EnclosingTunnel, lastTunnelAction, extendsTunnel, playerTransform);
        bool isFollowing = !isIntersecting && !extendsTunnel;

        if (isFollowing)
        {
            return false;
        }
        else
        {
            Ring prevRing = GetPrevRing(extendsTunnel, playerTransform, TunnelHeading);

            if (IsCreationValid(TunnelHeading, prevRing))
            {
                if (isIntersecting) // intersect
                {
                    Heading PrevHeading = PrevHeadingDict[playerTransform];

                    Debug.Log("TunnelAction Intersect");
                    GameObject prevSegment = TunnelManager.Instance.GetGameObjectTunnel(playerTransform);
                    OnIntersectTunnel?.Invoke(playerTransform, prevSegment, TunnelHeading, PrevHeading, otherTunnels, extendsTunnel, prevRing);
                    LastTunnelActionDict[playerTransform] = Action.Intersect;
                    return true;
                }
                else
                {
                    Debug.Log("Tunnel Action Create");
                    OnCreateTunnel?.Invoke(playerTransform, extendsTunnel, TunnelHeading, prevRing);
                    LastTunnelActionDict[playerTransform] = Action.Create;
                    return true;
                }
            }
            else
            {
                Debug.Log("Tunnel segment is not long enough");
            }
        }
        return false;
    }

    public bool IsCreationValid(Heading TunnelHeading, Ring prevRing)
    {
        float dist = Vector3.Distance(TunnelHeading.position, prevRing.GetCenter());

        return dist >= TunnelManager.minSegmentLength;
    }

    Ring GetPrevRing(bool extendsTunnel, Transform playerTransform, Heading TunnelHeading)
    {
        Ring prevRing;

        if (extendsTunnel)
        {
            prevRing = SegmentManager.Instance.GetSegmentFromTransform(playerTransform)?.GetEndRing();

            if (prevRing == null) // initialize the previous Ring
            {
                prevRing = RingManager.Instance.Create(TunnelHeading.forward, playerTransform.position);
            }
        }
        else
        {
            Heading PrevHeading = PrevHeadingDict[playerTransform];

            prevRing = RingManager.Instance.Create(TunnelHeading.forward, PrevHeading.position);
        }

        return prevRing;
    }

    /// <summary>
    /// Detect if a player is leaving an existing tunnel, or entering a new tunnel
    /// </summary>
    /// <param name="nextTunnel"></param>
    /// <param name="lastTunnelAction"></param>
    /// <returns></returns>
    bool IsIntersect(GameObject nextTunnel, Action lastTunnelAction, bool extendsTunnel, Transform playerTransform)
    {
        GameObject curTunnel = TunnelManager.Instance.GetGameObjectTunnel(playerTransform);

        if (nextTunnel != null)
        {
            // next tunnel segment is a new intersecting segment that is not already connected to current tunnel player is in
            return curTunnel != nextTunnel && !SegmentManager.Instance.IsTunnelsConnected(curTunnel, nextTunnel);
        }
        else
        {
            return !extendsTunnel;
        }
    }

    private void OnDisable()
    {
        Agent.OnDig -= TunnelAction;
    }
}
