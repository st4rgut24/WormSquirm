using System;
using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls a slider with options for players to choose from
/// </summary>
public class Slider
{
    private SimpleScrollSnap scrollSnap;
    private Dictionary<int, GameObject> UIPrefabDict;
    protected Dictionary<string, Text> PanelDict; // <tags of the gameobject, Panel text UI displayed in Slider>

    /// <summary>
    /// A slider base class to interface with the scrollSnap component
    /// </summary>
    /// <param name="scrollSnap">the scroll snap UI</param>
    /// <param name="UIPrefabDict">dictionary to look up prefabs that will be inserted into the slider</param>
    public Slider(SimpleScrollSnap scrollSnap, Dictionary<int, GameObject> UIPrefabDict)
	{
        this.scrollSnap = scrollSnap;
        this.UIPrefabDict = UIPrefabDict;
        PanelDict = new Dictionary<string, Text>();
    }

    /// <summary>
    /// Initialize the sliders with some objects at the beginning of the game
    /// </summary>
    /// <param name="initialObjects">initial ui objects</param>
    public void InitObjects(int[] initialObjects)
    {
        foreach (int initObjectId in initialObjects)
        {
            AddObject(initObjectId);
        }
    }

    /// <summary>
    /// Adds a gameobject with a matching type to Slider
    /// </summary>
    /// <param name="type">id of the gameobject's ui</param>
    public void AddObject(int type)
    {
        GameObject objectUI = this.UIPrefabDict[type];
        AddObjectUI(objectUI);
    }

    /// <summary>
    /// Removes a gameobject with a matching type to Slider
    /// </summary>
    /// <param name="type">id of the gameobject's ui</param>
    public void RemoveObject(int type)
    {
        GameObject objectUI = this.UIPrefabDict[type];
        RemoveObjectUI(objectUI);
    }

    /// <summary>
    /// Removes an object ui from list of slides
    /// </summary>
    /// <param name="obj">an existing gameobject</param>
    private void RemoveObjectUI(GameObject obj)
	{
		scrollSnap.RemoveObject(obj);
    }

    /// <summary>
    /// Adds an object ui from list of slides
    /// </summary>
    /// <param name="obj">an new gameobject</param>
    private void AddObjectUI(GameObject obj)
	{
        GameObject panelGo = scrollSnap.AddToBack(obj);
        Text panelText = panelGo.GetComponentInChildren<Text>();
        PanelDict[panelGo.tag] = panelText;
    }
}

