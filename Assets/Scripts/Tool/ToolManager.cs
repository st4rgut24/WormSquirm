using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using DanielLochner.Assets.SimpleScrollSnap;

public enum ToolType
{
    Pickaxe,
    Crossbow,
    Mace
}

public class ToolManager : Singleton<ToolManager>
{

    public RectTransform WeaponCanvas;

    public GameObject PickaxePrefab;
    public GameObject MacePrefab;
    public GameObject CrossbowPrefab;

    static Dictionary<ToolType, GameObject> WeaponPrefabDict = new Dictionary<ToolType, GameObject>();

    public GameObject toolboxGo;

    public Camera playerCamera;
    public MainPlayer player;

    Weapon activeTool = null;
    
    Transform playerTransform;

    private void OnEnable()
    {
        PlayerManager.SpawnMainPlayerEvent += OnSpawnMainPlayer;
        SimpleScrollSnap.SelectedItemEvent += OnSelectedItem;
    }

    public void OnSpawnMainPlayer(GameObject mainPlayerGo)
    {
        player = mainPlayerGo.GetComponent<MainPlayer>();

        playerTransform = mainPlayerGo.transform;
        Camera[] cameras = mainPlayerGo.GetComponentsInChildren<Camera>();
        playerCamera = cameras[0];

        InitWeaponPrefabDict(); // depends on player being initialized   

    }

    public ToolType GetToolTypeFromTag(string tag)
    {
        switch (tag)
        {
            case Consts.MaceTag:
                return ToolType.Mace;
            case Consts.PickaxeTag:
                return ToolType.Pickaxe;
            case Consts.CrossbowTag:
                return ToolType.Crossbow;
            default:
                return ToolType.Pickaxe;
        }
    }

    public void InitWeaponPrefabDict()
    {
        WeaponPrefabDict[ToolType.Pickaxe] = InitWeaponPrefab(PickaxePrefab);
        WeaponPrefabDict[ToolType.Crossbow] = InitWeaponPrefab(CrossbowPrefab);
        WeaponPrefabDict[ToolType.Mace] = InitWeaponPrefab(MacePrefab);

    }

    public GameObject InitWeaponPrefab(GameObject WeaponPrefab)
    {
        GameObject WeaponGo = Instantiate(WeaponPrefab, player.handTransform, false);
        WeaponGo.SetActive(false);
        return WeaponGo;
    }

    public void PlayWeaponAnim(ToolType type, bool isPaused)
    {
        player.PlayWeayponAnimation(type, isPaused);
    }

    public void StopWeaponAnim(ToolType type, bool isPaused)
    {
        if (player != null)
        {
            player.StopWeaponAnimation(type, isPaused);
        }
    }

    public GameObject GetWeaponPrefab(ToolType type)
    {
        return WeaponPrefabDict[type];
    }

    /// <summary>
    /// Time the weapon's effects (eg dealing damage, digging) with this animation event callback
    /// </summary>
    public void UseOnTrigger()
    {
        activeTool.Use();
    }

    public void OnSelectedItem(GameObject item)
    {
        if (TransformUtils.IsTransformMatchTags(item.transform, Consts.ToolTags))
        {
            EquipTool(item.tag);
        }
    }

    /// <summary>
    /// Enable the active tool and disable the previously active tool
    /// </summary>
    /// <param name="equippedTool">active tool</param>
    void SwapTool(GameObject equippedTool)
    {
        if (activeTool != null)
        {
            activeTool.gameObject.SetActive(false);
            player.StopWeaponAnimation(activeTool.toolType, false);
        }

        activeTool = equippedTool.GetComponent<Weapon>();
        activeTool.isEquipped = true;

        equippedTool.SetActive(true);
    }



    /// <summary>
    /// Disengage the tool so that it does not interact with its surroundings unless it's being used
    /// </summary>
    public void DisengageTool()
    {
        activeTool.DisengageWeapon();
    }

    /// <summary>
    /// Equip a tool if not already equipped
    /// </summary>
    /// <param name="toolTag">tag of tool gameobject</param>
    public void EquipTool(string toolTag)
    {
        // TODO: play some animation (RunShoot for crossbow for example)
        if (activeTool == null || !activeTool.CompareTag(toolTag))
        {
            ToolType type = GetToolTypeFromTag(toolTag);
            GameObject toolGo = GetWeaponPrefab(type);
            SwapTool(toolGo);
            player.EquipTool(toolGo);
        }
    }

    private void OnDisable()
    {
        PlayerManager.SpawnMainPlayerEvent -= OnSpawnMainPlayer;
        SimpleScrollSnap.SelectedItemEvent -= OnSelectedItem;
    }
}

