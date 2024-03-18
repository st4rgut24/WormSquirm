using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using static UnityEditor.Rendering.CameraUI;

/// <summary>
/// Notifies other GameObjects of Tunnel Actions
/// </summary>
public class TunnelActionManager: Singleton<TunnelActionManager>
{
    Grid tunnelGrid;

    public static event Action<Transform, GameObject, Heading, bool, Ring, HitInfo> OnIntersectTunnel; // intersect an existing tunnel
    public static event Action<Transform, Heading, Ring> OnCreateTunnel; // create a new unobstructed tunnel
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
        //PlayerManager.SpawnMainPlayerEvent += OnInitTunnelForAgent;
    }

    private void Awake()
    {
        tunnelGrid = GameManager.Instance.GetGrid(GridType.Tunnel);
    }

    /// <summary>
    /// When an agent is first spawned (typically the main player), place that agent in a tunnel
    /// </summary>
    /// <param name="agent">The spawned agent</param>
    public void OnInitTunnelForAgent(GameObject agent)
    {
        TunnelAction(agent.transform, agent.transform.forward);
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
        
        Ring prevRing = GetPrevRing(extendsTunnel, playerTransform);

        try
        {
            if (CanDig(prevRing, playerTransform))
            {
                return DigNewTunnel(playerTransform, TunnelHeading, prevRing, extendsTunnel);
            }
            else
            {
                return false;
            }
        }
        catch(Exception e)
        {
            Debug.LogWarning("Unable to create new tunnel due to error: " + e.Message);

            return false;
        }
    }


    /// <summary>
    /// Dig a new tunnel
    /// </summary>
    /// <param name="playerTransform">Transform of digger</param>
    /// <param name="TunnelHeading">direction of new tunnel</param>
    /// <param name="prevRing">start of the new tunnel</param>
    /// <param name="extendsTunnel">whether this tunnel is extending an existing tunnel</param>
    /// <returns></returns>
    private bool DigNewTunnel(Transform playerTransform, Heading TunnelHeading, Ring prevRing, bool extendsTunnel)
    {
        List<GameObject> otherTunnels = tunnelGrid.GetGameObjects(TunnelHeading.position, 1, playerTransform);

        RayRing hitTestRayRing = new RayRing(prevRing, playerTransform.forward); // get rays for the vertices of a tunnel ring
        HitInfo hitInfo = TunnelUtils.GetHitInfoFromRays(hitTestRayRing.rays, otherTunnels, GameManager.Instance.agentOffset);

        // if hit detected, create a new ray that is directed at the center guideline of the hit tunnel

        Ray ray = new Ray(playerTransform.position, playerTransform.forward);
        Debug.DrawRay(ray.origin, ray.direction * GameManager.Instance.agentOffset, Color.red, 100);

        bool isIntersecting = IsIntersect(hitInfo, extendsTunnel, playerTransform);
        bool isFollowing = !isIntersecting && !extendsTunnel;

        if (isFollowing)
        {
            return false;
        }
        else
        {
            if (isIntersecting) // intersect
            {
                // Debug.Log("Tunnel Action Intersect");
                GameObject prevSegment = TunnelManager.Instance.GetGameObjectTunnel(playerTransform);
                OnIntersectTunnel?.Invoke(playerTransform, prevSegment, TunnelHeading, extendsTunnel, prevRing, hitInfo);
            }
            else
            {
                // Debug.Log("Tunnel Action Create");
                OnCreateTunnel?.Invoke(playerTransform, TunnelHeading, prevRing);
            }
            return true;
        }
    }

    private bool CanDig(Ring startRing, Transform diggerTransform)
    {
        Debug.Log("Distance to digging location " + Vector3.Distance(startRing.center, diggerTransform.position));
        return Vector3.Distance(startRing.center, diggerTransform.position) < Consts.MaxDistToDig;
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
        else // create ring at the point of intersection when bisecting from inside an existing tunnel 
        {
            Heading playerHeading = new Heading(playerTransform.position, playerTransform.forward);
            Vector3 intersectionPoint = segment.GetIntersectionPoint(playerHeading);
            // Debug.Log("intersection point is " + intersectionPoint);

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
            return nextTunnel.tag == Consts.TunnelTag && curTunnel != nextTunnel && !SegmentManager.Instance.IsTunnelsConnected(curTunnel, nextTunnel);
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
        //PlayerManager.SpawnMainPlayerEvent -= OnInitTunnelForAgent;
    }
}
