using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements.Experimental;
using static UnityEngine.Rendering.HableCurve;
using Unity.VisualScripting;

/// <summary>
/// Manages the intersection of tunnels
/// </summary>
public class TunnelIntersectorManager : Singleton<TunnelIntersectorManager>
{
    public static event Action<Transform, SegmentGo, GameObject, List<GameObject>> OnAddIntersectedTunnelSuccess;
    public static event Action<Transform> OnAddIntersectedTunnelFailure;

    public int rayRings = 3; // The bigger this number is, the smaller the interval
    public int offsetMultiple = 3; // How many units the intersection ray should be offset from the intersecting faces
    public float intersectBuffer = .25f;

    TunnelMake tunnelMaker;
    Grid tunnelGrid;

    TunnelProps _props;
    int _ringVertices;
    float _holeRadius;

    int _rayInterval; // the smaller the ray interval, the more precise the collision tests during intersection

    private void OnEnable()
    {
        TunnelActionManager.OnIntersectTunnel += IntersectAction;
    }

    void Awake()
    {
        tunnelMaker = GameObject.FindObjectOfType<TunnelMake>();

    }

    private void Start()
    {
        tunnelGrid = GameManager.Instance.GetGrid(GridType.Tunnel);
        _props = TunnelManager.Instance.defaultProps;
        _rayInterval = Mathf.FloorToInt(_props.TunnelRadius / rayRings);

        _ringVertices = _props.TunnelSides + 5;
        _holeRadius = _props.TunnelRadius / 2;
    }

    /// <summary>
    /// Get the Tunnels that will be intersected
    /// </summary>
    /// <param name="playerTransform">The player transform</param>
    /// <param name="otherTunnels">Other tunnels in the vicinity of the active tunnel</param>
    /// <param name="prevTunnel">Previous tunnel segment belonging to the active tunnel</param>
    /// <param name="extendsTunnel">The player is extending a tunnel</param>
    /// <param name="heading">The directional info of the tunnel</param>
    /// <param name="hitInfo">info about the hit object</param>
    void IntersectAction(Transform playerTransform, GameObject prevTunnel, Heading heading, bool extendsTunnel, Ring prevRing, HitInfo hitInfo)
    {
        Heading intersectHeading = hitInfo != null ? GetHitHeading(hitInfo, playerTransform) : heading;


        //if (hitInfo != null)
        //{
        //    ModifyHitPosition()
        //    heading.position = hitInfo.hitCoord; // the end of the intersecting segment will be the point of intersection obtained from hit info
        //}

        Vector3 center = TunnelUtils.GetCenterPoint(prevRing.GetCenter(), intersectHeading.position);
        List<GameObject> nearbyTunnels = tunnelGrid.GetGameObjects(center, 1);
        Debug.Log("There are " + nearbyTunnels.Count + " tunnels with the viciting of position " + center);

        RayRing endRayRing = new RayRing(_ringVertices, -intersectHeading.forward, intersectHeading.position, offsetMultiple, _rayInterval, _holeRadius);
        List<Ray> intersectingRays = new List<Ray>(endRayRing.rays);

        if (!extendsTunnel) // tunnel faces may be deleted from the start of a segment if the intersecting segment bisects the current segment
        {
            RayRing startRayRing = new RayRing(_ringVertices, intersectHeading.forward, prevRing.GetCenter(), offsetMultiple, _rayInterval, _holeRadius);
            intersectingRays.AddRange(startRayRing.rays);
        }


        Intersect(playerTransform, prevTunnel, nearbyTunnels, intersectingRays, intersectHeading, prevRing);
    }

    void Intersect(Transform playerTransform, GameObject prevTunnel, List<GameObject> otherTunnels, List<Ray> rays, Heading heading, Ring prevRing)
    {
        SegmentGo projectedSegment = null;

        try
        {
            projectedSegment = tunnelMaker.GrowTunnel(playerTransform, heading, prevRing);
            projectedSegment.IntersectStartCap(); // start of intersected tunnel segment will have a hole in it

            List<GameObject> intersectedTunnels = TunnelUtils.GetIntersectedObjects(projectedSegment.getTunnel(), otherTunnels, intersectBuffer);

            ValidateIntersection(intersectedTunnels, heading);
            List<GameObject> deletedTunnels = DeleteTunnels(intersectedTunnels, rays);

            OnAddIntersectedTunnelSuccess?.Invoke(playerTransform, projectedSegment, prevTunnel, deletedTunnels);
        } catch (Exception e) {
            Debug.LogError(e.Message);

            if (projectedSegment != null)
            {
                projectedSegment.Destroy();
            }
            OnAddIntersectedTunnelFailure?.Invoke(playerTransform);
        }
    }

    /// <summary>
    /// Delete tunnels at the intersection points
    /// </summary>
    /// <param name="intersectedTunnels">list of candidate tunnels to delete faces from</param>
    /// <param name="rays">rays that represent the intersection points</param>
    /// <returns>list of tunnels that had their faces deleted</returns>
    List<GameObject> DeleteTunnels(List<GameObject> intersectedTunnels, List<Ray> rays)
    {
        List<GameObject> deletedTunnels = new List<GameObject>();

        intersectedTunnels.ForEach((tunnel) =>
        {
            TunnelDelete tunnelDelete = new TunnelDelete(tunnel, rays);
            bool isDeleted = tunnelDelete.DeleteTunnel();

            if (isDeleted)
            {
                deletedTunnels.Add(tunnel);
            }
        });

        return deletedTunnels;
    }

    /// <summary>
    /// Create a new heading so that the intersecting tunnel intersects squarely with the other tunnel
    /// </summary>
    /// <param name="hitInfo">Initial hit info</param>
    /// <returns>Heading related to the final hit position</returns>
    Heading GetHitHeading(HitInfo hitInfo, Transform playerTransform)
    {
        // shoot a new ray to the modified position
        Vector3 hitPosition = hitInfo.hitCoord;
        Segment segment = SegmentManager.Instance.GetSegmentFromObject(hitInfo.hitGo);

        Vector3 intersectPoint = segment.GetClosestPointToCenterline(hitPosition);
        Vector3 startPoint = playerTransform.position;

        Vector3 rayDir = intersectPoint - startPoint;
        Ray hitRay = new Ray(startPoint, rayDir);
        HitInfo modHitInfo = TunnelUtils.GetHitInfoFromRay(hitRay, hitInfo.hitGo);

        Heading intersectHeading = new Heading(modHitInfo.hitCoord, rayDir.normalized);

        Debug.Log("hit coord " + modHitInfo.hitCoord);
        return intersectHeading;
    }

    /// <summary>
    /// Is the intersection valid, ie not affected by any constraints
    /// </summary>
    /// <param name="intersectedTunnels">tunnels that would be intersected</param>
    /// <param name="intersectionHeading">positional info of the intersection point</param>
    bool ValidateIntersection(List<GameObject> intersectedTunnels, Heading intersectionHeading)
    {
        Vector3 intersectPoint = intersectionHeading.position;
        Debug.Log("intersect position " + intersectPoint);
        // check if intersected tunnels have caps (meaning they are not continuous intermediary segments)
        // check the distance from the intersection point to any caps
        // if the distance is less than minimum allowed, throw an exception
        // in the catch{}
        //      1. destroy the intersecting tunnel
        //      2. emit an event about creation error (listened for by digger)
        intersectedTunnels.ForEach((intersectedTunnel) =>
        {
            Segment segment = SegmentManager.Instance.GetSegmentFromObject(intersectedTunnel);
            bool intersectsCaps = IsIntersectingCap(intersectPoint, segment.GetStartCap()) || IsIntersectingCap(intersectPoint, segment.GetEndCap());

            if (intersectsCaps)
            {
                throw new Exception("Invalid intersection, intersects caps");
            }
        });

        return true;
    }

    /// <summary>
    /// Check if a new point of intersection is too close to another ring 
    /// </summary>
    /// <param name="intersectionPoint">point of intersection</param>
    /// <param name="center">center of a ring</param>
    /// <returns>true if ring center intersects</returns>
    bool IsIntersectingCap(Vector3 intersectionPoint, Cap cap)
    {
        if (cap.HasCap())
        {
            Ring ring = cap.ring;
            float hypotenuse = Vector3.Distance(intersectionPoint, ring.center);
            Debug.DrawRay(intersectionPoint, ring.center - intersectionPoint, Color.red, 30000);

            float side = ring.radius;
            float height = Mathf.Sqrt(Mathf.Pow(hypotenuse, 2) - Mathf.Pow(side, 2));
            float minDist = ring.radius + SegmentManager.Instance.MinDistFromCap;
            Debug.Log("intersection point is " + height + " units from cap center. min dist is " + minDist + " Units");
            return height <= minDist; // if it is intersecting, then height is less than minimum
        }
        else
        {
            return false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDisable()
    {
        TunnelActionManager.OnIntersectTunnel -= IntersectAction;
    }
}