using System;
using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValuablesSlider : Slider
{
    public ValuablesSlider(SimpleScrollSnap scrollSnap, Dictionary<int, GameObject> ValuablesUIPrefabs) : base(scrollSnap, ValuablesUIPrefabs)
    {

    }

    /// <summary>
    /// Update the text to the gameobject in the UI panel
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="text"></param>
    public void SetSlideText(string tag, string text)
    {
        if (PanelDict.ContainsKey(tag))
        {
            Text panelText = PanelDict[tag];
            panelText.text = text;
        }
        else
        {
            Debug.LogWarning("The panel for tag " + tag + " has not been created yet");
        }
    }

    /// <summary>
    /// Adds a newly created key to the slider
    /// </summary>
    /// <param name="jewel">the new key</param>
    public void OnKeyCreated(Jewel jewel)
    {
        AddObject((int)jewel.type);
    }

    /// <summary>
    /// Removes a key to the slider
    /// </summary>
    /// <param name="jewel">the key to remove from slider UI</param>
    public void OnKeyDestroyed(Jewel jewel)
    {
        RemoveObject((int)jewel.type);
    }
}

