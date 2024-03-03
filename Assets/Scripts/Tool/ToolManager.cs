using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public enum ToolType
{
    Pickaxe,
    Crossbow,
    Chain
}

public class ToolManager : Singleton<ToolManager>
{
    public GameObject PickaxePrefab;
    public GameObject ChainPrefab;
    public GameObject CrossbowPrefab;

    static Dictionary<ToolType, GameObject> WeaponPrefabDict = new Dictionary<ToolType, GameObject>();

    public GameObject toolboxGo;

    public RectTransform crosshairUI;
    public Camera playerCamera;
    MainPlayer player;

    Weapon activeTool = null;
    
    Transform playerTransform;

    // Use this for initialization
    void Start()
    {
        PlayerManager pm = AgentManager.Instance.playerManager;

        GameObject mainPlayerGo = pm.GetMainPlayer();
        player = mainPlayerGo.GetComponent<MainPlayer>();

        playerTransform = mainPlayerGo.transform;
        Camera[] cameras = mainPlayerGo.GetComponentsInChildren<Camera>();
        playerCamera = cameras[0];

        InitWeaponPrefabDict(); // depends on player being initialized

        GameObject pickaxeTool = CreateTool(ToolType.Pickaxe); // depends on WeaponPrefabDict being initialized
        Equip(pickaxeTool);
    }

    public void InitWeaponPrefabDict()
    {
        WeaponPrefabDict[ToolType.Pickaxe] = InitWeaponPrefab(PickaxePrefab);
        // TODO: create the prefabs for other weapons below
        //WeaponPrefabDict[ToolType.Crossbow] = InitWeaponPrefab(CrossbowPrefab);
        //WeaponPrefabDict[ToolType.Chain] = InitWeaponPrefab(ChainPrefab);

    }

    public GameObject InitWeaponPrefab(GameObject WeaponPrefab)
    {
        GameObject WeaponGo = Instantiate(WeaponPrefab, player.handTransform, false);
        Weapon weapon = WeaponGo.GetComponent<Weapon>();
        WeaponGo.SetActive(false);
        return WeaponGo;
    }

    public void PlayWeaponAnim(ToolType type)
    {
        player.SetWeaponAnimation(type);
    }

    public GameObject GetWeaponPrefab(ToolType type)
    {
        return WeaponPrefabDict[type];
    }

    /// <summary>
    /// A certain event in the animation is required to trigger the effects of the weapon (eg attack, dig)
    /// </summary>
    public void UseOnTrigger()
    {
        activeTool.Use();
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

        activeTool = equippedTool.GetComponent<Weapon>();
        equippedTool.SetActive(true);
    }

    GameObject CreateTool(ToolType type)
    {
        GameObject toolGo = GetWeaponPrefab(type);
        player.EquipTool(toolGo);

        return toolGo;
    }
}

