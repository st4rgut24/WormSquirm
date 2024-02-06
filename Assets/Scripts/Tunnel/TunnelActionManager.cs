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

    public static event Action<Transform, GameObject, Heading, bool, Ring, HitInfo> OnIntersectTunnel; // intersect an existing tunnel
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
        AgentManager.OnSpawn += OnSpawn;
    }

    private void Awake()
    {
        tunnelGrid = GameManager.Instance.GetGrid(GridType.Tunnel);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    /// <summary>
    /// Set a reference point for the creation of the first tunnel when an agent is spawned
    /// </summary>
    /// <param name="transform">agent transform</param>
    public void OnSpawn(Transform transform)
    {
        Heading initHeading = DirectionUtils.GetHeading(transform.position, transform.forward, 0);
        //PrevHeadingDict[transform] = initHeading;
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
        }

    }

    /// <summary>
    /// Create Tunnel Segments
    /// </summary>
    /// <param name="playerTransform">transform of the player</param>
    /// <param name="TunnelHeading">Head of the projected tunnel using player coordinates as reference</param>
    public bool CreateTunnel(Transform playerTransform, Heading TunnelHeading)
    {
        bool extendsTunnel = SegmentManager.Instance.IsExtendingTunnel(playerTransform);
        Debug.Log("is extending tunnel");
        List<GameObject> otherTunnels = tunnelGrid.GetGameObjects(TunnelHeading.position, 1);

        HitInfo hitInfo = TunnelUtils.getHitObject(playerTransform, otherTunnels);

        //Ray ray = new Ray(playerTransform.position, playerTransform.forward);
        //Debug.DrawRay(ray.origin, ray.direction * GameManager.Instance.agentOffset, Color.red, 100);

        bool isIntersecting = IsIntersect(hitInfo, extendsTunnel, playerTransform);
        bool isFollowing = !isIntersecting && !extendsTunnel;

        if (isFollowing)
        {
            return false;
        }
        else
        {
            Ring prevRing = GetPrevRing(extendsTunnel, playerTransform); 

            if (isIntersecting) // intersect
            {

                Debug.Log("TunnelAction Intersect");
                GameObject prevSegment = TunnelManager.Instance.GetGameObjectTunnel(playerTransform);
                OnIntersectTunnel?.Invoke(playerTransform, prevSegment, TunnelHeading, extendsTunnel, prevRing, hitInfo);
            }
            else
            {
                Debug.Log("TunnelAction Create");
                OnCreateTunnel?.Invoke(playerTransform, extendsTunnel, TunnelHeading, prevRing);
            }

            return true;
        }
    }

    public bool IsCreationValid(Heading TunnelHeading, Ring prevRing)
    {
        float dist = Vector3.Distance(TunnelHeading.position, prevRing.GetCenter());

        return dist >= TunnelManager.minSegmentLength;
    }

    Ring GetPrevRing(bool extendsTunnel, Transform playerTransform)
    {
        Ring prevRing;
        Segment segment = AgentManager.Instance.GetSegment(playerTransform);

        if (extendsTunnel)
        {
            prevRing = segment?.GetEndRing();

            if (prevRing == null) // initialize the previous Ring
            {
                prevRing = RingFactory.Create(playerTransform.forward, playerTransform.position);
            }
        }
        else
        {
            Heading playerHeading = new Heading(playerTransform.position, playerTransform.forward);
            Vector3 intersectionPoint = segment.GetIntersectionPoint(playerHeading);
            prevRing = RingFactory.Create(playerTransform.forward, intersectionPoint);
        }

        return prevRing;
    }

    /// <summary>
    /// Detect if a player is leaving an existing tunnel, or entering a new tunnel
    /// </summary>
    /// <param name="nextTunnel"></param>
    /// <param name="lastTunnelAction"></param>
    /// <returns></returns>
    bool IsIntersect(HitInfo hitInfo, bool extendsTunnel, Transform playerTransform)
    {
        GameObject curTunnel = TunnelManager.Instance.GetGameObjectTunnel(playerTransform);

        if (hitInfo != null)
        {
            GameObject nextTunnel = hitInfo.hitGo;
            // intersects is true, if next tunnel segment is a new intersecting segment that is not already connected to current tunnel player is in
            return curTunnel != nextTunnel && !SegmentManager.Instance.IsTunnelsConnected(curTunnel, nextTunnel);
        }
        else
        {
            // if there is no tunnel in the projected next segment location,
            // intersection occurs if leaving an existing segment (T shape),
            // when not extending the current tunnel
            return !extendsTunnel;
        }
    }

    private void OnDisable()
    {
        Agent.OnDig -= TunnelAction;
        AgentManager.OnSpawn -= OnSpawn;
    }
}
