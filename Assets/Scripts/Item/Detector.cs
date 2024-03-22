using System;
using System.Collections.Generic;
using UnityEngine;
using static BotManager;

/// <summary>
/// Provide player with hints on where to look for valuables
/// </summary>
public class Detector : MonoBehaviour
{
    public LineRenderer digLineRenderer;

    private Agent detective;
    private Transform detectiveTransform;
    Vector3 prevPosition; // last position the detective was in

    private float distance;

    Transform target;

    private void OnEnable()
    {
        // TODO: This is temporary just for testing the line detection system.
        // we want this event to be handled by the Valuables ToolSlider and when
        // the user has a slide selected, it should trigger the code in OnKeyCreated
        GateManager.CreateKeyEvent += OnKeyCreated;
    }

    private void Awake()
    {
        prevPosition = DefaultUtils.DefaultVector3;
    }

    public void Init(Agent player)
    {
        detective = player;
        detectiveTransform = player.transform;
    }

    // TODO: This is temporary just for testing the line detection system.
    // we want this event to be handled by the Valuables ToolSlider and when
    // the user has a slide selected, it should trigger this code here
    public void OnKeyCreated(GameObject keyGo)
    {
        target = keyGo.transform;
    }

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }

    private bool DetectivePositionIsUpdated()
    {
        return !detectiveTransform.Equals(prevPosition);
    }

	void Update()
	{
        if (detective != null && target != null && DetectivePositionIsUpdated())
        {            
            distance = Vector3.Distance(detectiveTransform.position, target.position);
            // TODO: Send the distance to ScrollSnap for textual clues

            // TODO: Place the detector gizmo in the correct location
            digLineRenderer.SetPositions(new Vector3[] { detectiveTransform.position, target.position });

            prevPosition = detectiveTransform.position;
        }
    }

    private void OnDisable()
    {
        GateManager.CreateKeyEvent -= OnKeyCreated;
    }
}

