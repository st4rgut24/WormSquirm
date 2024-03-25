using System;
using System.Collections.Generic;
using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine;

/// <summary>
/// Provide player with hints on where to look for keys
/// </summary>
public class Detector : MonoBehaviour
{
    public static event Action<string, float> DetectDistanceEvent; // <target tag, distance to target>

    HashSet<string> FoundSet; // set of object tags that have been found and should be ignored by detector

    public LineRenderer digLineRenderer;

    private const float detectThreshold = 1; // distance for detective to travel before recalculating distance to target

    private Agent detective;
    private Transform detectiveTransform;
    Vector3 prevPosition; // last position the detective was in

    private float distance;
    Transform target;

    private void OnEnable()
    {
        Jewel.RepositionJewelEvent += OnKeyRepositioned;
        Jewel.CollectJewelEvent += OnKeyFound;
        SimpleScrollSnap.SelectedItemEvent += OnSelectedItem;
    }

    private void Awake()
    {
        FoundSet = new HashSet<string>();
        prevPosition = DefaultUtils.DefaultVector3;
    }

    public void Init(Agent player)
    {
        detective = player;
        detectiveTransform = player.transform;
    }

    /// <summary>
    /// If the key has been moved, update the target if it matches the key
    /// </summary>
    /// <param name="key">The moved key</param>
    public void OnKeyRepositioned(Jewel key)
    {
        if (target.CompareTag(key.tag))
        {
            UpdateDetectPosition();
        }
    }

    /// <summary>
    /// Don't target keys that have been found.
    /// </summary>
    /// <param name="jewel">the found key</param>
    /// <param name="segment">segment the key was found in</param>
    public void OnKeyFound(Jewel key, Segment segment)
    {
        FoundSet.Add(key.tag);
        DropTarget(key.tag);
    }

    private bool KeyIsMissing(string tag)
    {
        return !FoundSet.Contains(tag);
    }

    /// <summary>
    /// Check whether GameObject is representative of key that player is looking for
    /// </summary>
    /// <param name="obj">item gameobject</param>
    /// <returns></returns>
    private bool ItemIsMissingKey(GameObject obj)
    {
        return TransformUtils.IsTransformMatchTags(obj.transform, Consts.KeyTags) && KeyIsMissing(obj.tag);
    }

    /// <summary>
    /// Triggered when user selects an item in the UI. The gameobject is the UI,
    /// so we neeed to find the coresponding gameobject, stored previously
    /// </summary>
    /// <param name="selectedItemUI">gameobject living in the UI slider</param>
    public void OnSelectedItem(GameObject selectedItemUI)
    {
        if (ItemIsMissingKey(selectedItemUI))
        {
            string uiTag = selectedItemUI.tag;
            GameObject gameKeyGo = SceneItemManager.Instance.GetSceneObject(uiTag);
            SetTarget(gameKeyGo.transform); // update line distance
            UpdateTarget();
        }
    }

    /// <summary>
    /// Stop detecting distance to target
    /// </summary>
    /// <param name="droppedTargetTag">target to stop tracking</param>
    private void DropTarget(string droppedTargetTag)
    {
        if (target != null && target.CompareTag(droppedTargetTag))
        {
            target = null; // dont compute distance to target anymore
            digLineRenderer.enabled = false; // disappear the target line
        }
    }

    public void SetTarget(Transform targetTransform)
    {
        digLineRenderer.enabled = true; // show the target line (may already be active)
        target = targetTransform;
    }

    /// <summary>
    /// Update target position after moving a certain amount
    /// </summary>
    /// <returns>true if should update distance to target</returns>
    private void UpdateDetectPosition()
    {
        UpdateTarget();
        prevPosition = detectiveTransform.position;
    }

	void Update()
	{
        if (detectiveTransform != null && Vector3.Distance(detectiveTransform.position, prevPosition) > detectThreshold)
        {
            UpdateDetectPosition();
        }
    }

    /// <summary>
    /// Update target distance
    /// </summary>
    private void UpdateTarget()
    {
        if (target != null)
        {
            distance = Vector3.Distance(detectiveTransform.position, target.position);

            DetectDistanceEvent?.Invoke(target.tag, distance);

            Vector3 detectPos = detectiveTransform.position - transform.up;
            digLineRenderer.SetPositions(new Vector3[] { detectPos, target.position });

            prevPosition = detectiveTransform.position;
        }
    }

    private void OnDisable()
    {
        Jewel.RepositionJewelEvent -= OnKeyRepositioned;
        Jewel.CollectJewelEvent -= OnKeyFound;
        SimpleScrollSnap.SelectedItemEvent -= OnSelectedItem;
    }
}

