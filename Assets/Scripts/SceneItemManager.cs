using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Map the UI GameObjects to In Scene Item GameObjects
/// This is useful for looking up objects corresponding to the selected Slider UI
/// </summary>
public class SceneItemManager : Singleton<SceneItemManager>
{
    public Dictionary<string, GameObject> SceneGameObjectDict; // <tag of the object, GameObject in the scene>

    private void Awake()
    {
        SceneGameObjectDict = new Dictionary<string, GameObject>();
    }

    public GameObject AddSceneObject(GameObject obj)
    {
        //if (SceneGameObjectDict.ContainsKey(obj.tag))
        //{
        //    throw new System.Exception("Scene object already added, Duplicate items not supported");
        //}
        SceneGameObjectDict[obj.tag] = obj;

        return obj;
    }

    public void RemoveSceneObject(string tag)
    {
        SceneGameObjectDict.Remove(tag);
    }

    public GameObject GetSceneObject(string tag)
    {
        return SceneGameObjectDict[tag];
    }
}

