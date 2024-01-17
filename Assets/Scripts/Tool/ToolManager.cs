using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ToolType
{
    Pickaxe
}

public class ToolManager : Singleton<ToolManager>
{
    public GameObject toolboxGo;
    Dictionary<ToolType, GameObject> toolbox;

    List<GameObject> tools;

    public RectTransform crosshairUI;
    public Camera playerCamera;

    Tool activeTool = null;

    Transform playerTransform;
    // Use this for initialization
    void Start()
    {
        toolbox = new Dictionary<ToolType, GameObject>();
        tools = new List<GameObject>();

        GameObject mainPlayerGo = PlayerManager.Instance.GetMainPlayer();
        playerTransform = mainPlayerGo.transform;
        playerCamera = playerTransform.GetChild(0).GetComponent<Camera>();

        GameObject pickaxeTool = CreateTool(ToolType.Pickaxe);
        Equip(pickaxeTool);
    }

    /// <summary>
    /// Enable the active tool and disable the previously active tool
    /// </summary>
    /// <param name="equippedTool">active tool</param>
    void Equip(GameObject equippedTool)
    {
        if (activeTool != null)
        {
            activeTool.gameObject.SetActive(false);
        }

        activeTool = equippedTool.GetComponent<Tool>();
        equippedTool.SetActive(true);
    }

    GameObject CreateTool(ToolType type)
    {
        GameObject toolGo = new GameObject();
        if (type == ToolType.Pickaxe)
        {
            toolGo.AddComponent<Pickaxe>();
        }
        else
        {
            Debug.LogError("No type found that matches " + type);
            return null;
        }

        toolGo.transform.parent = toolboxGo.transform;

        toolbox.Add(ToolType.Pickaxe, toolGo);
        tools.Add(toolGo);

        return toolGo;
    }

    public GameObject GetTool(ToolType type)
    {
        if (toolbox.ContainsKey(type))
        {
            return toolbox[type];
        }
        else
        {
            Debug.LogError("Tool does not exist with type " + type);
            return null;
        }
    }

    public void Use()
    {
        if (activeTool != null)
        {
            activeTool.Use();
        }
    }
}

