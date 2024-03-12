using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using DanielLochner.Assets.SimpleScrollSnap;

public enum ToolType
{
    Pickaxe,
    Crossbow,
    Chain
}

public class ToolManager : Singleton<ToolManager>
{

    public RectTransform WeaponCanvas;

    SimpleScrollSnap scrollSnap;
    ToolSlider toolSlider;
    public GameObject PickaxeUIPrefab;
    public GameObject CrossbowUIPrefab;

    public GameObject PickaxePrefab;
    public GameObject ChainPrefab;
    public GameObject CrossbowPrefab;

    static Dictionary<ToolType, GameObject> WeaponPrefabDict = new Dictionary<ToolType, GameObject>();
    static Dictionary<ToolType, GameObject> WeaponUIPrefabDict = new Dictionary<ToolType, GameObject>();

    public GameObject toolboxGo;

    public Camera playerCamera;
    public MainPlayer player;

    Weapon activeTool = null;
    
    Transform playerTransform;

    private void OnEnable()
    {
        //PlayerManager.SpawnMainPlayerEvent += OnSpawnMainPlayer;
        SimpleScrollSnap.SelctedItemEvent += OnSelectedItem;
    }

    private void Awake()
    {
        GameObject mainPlayerGo = PlayerManager.Instance.GetMainPlayer();
        player = mainPlayerGo.GetComponent<MainPlayer>();

        playerTransform = mainPlayerGo.transform;
        Camera[] cameras = mainPlayerGo.GetComponentsInChildren<Camera>();
        playerCamera = cameras[0];

        InitWeaponPrefabDict(); // depends on player being initialized   
    }

    // Use this for initialization
    void Start()
    {

        //EquipTool(Consts.PickaxeTag); // depends on WeaponPrefabDict being initialized
    }

    ///// <summary>
    ///// Initialize whatever depends on main player
    ///// </summary>
    ///// <param name="mainPlayer"></param>
    //public void OnSpawnMainPlayer(GameObject mainPlayer)
    //{
    //    InitWeaponUIPrefabDict();

    //    scrollSnap = mainPlayer.GetComponentInChildren<SimpleScrollSnap>();
    //    toolSlider = new ToolSlider(scrollSnap, WeaponUIPrefabDict);

    //}

    public ToolType GetToolTypeFromTag(string tag)
    {
        switch (tag)
        {
            case Consts.ChainTag:
                return ToolType.Chain;
            case Consts.PickaxeTag:
                return ToolType.Pickaxe;
            case Consts.CrossbowTag:
                return ToolType.Crossbow;
            default:
                return ToolType.Pickaxe;
        }
    }

    //public void InitWeaponUIPrefabDict()
    //{
    //    WeaponUIPrefabDict[ToolType.Pickaxe] = Instantiate(PickaxeUIPrefab);
    //    WeaponUIPrefabDict[ToolType.Crossbow] = Instantiate(CrossbowUIPrefab);
    //}

    public void InitWeaponPrefabDict()
    {
        WeaponPrefabDict[ToolType.Pickaxe] = InitWeaponPrefab(PickaxePrefab);
        WeaponPrefabDict[ToolType.Crossbow] = InitWeaponPrefab(CrossbowPrefab);
        //WeaponPrefabDict[ToolType.Chain] = InitWeaponPrefab(ChainPrefab);

    }

    public GameObject InitWeaponPrefab(GameObject WeaponPrefab)
    {
        GameObject WeaponGo = Instantiate(WeaponPrefab, player.handTransform, false);
        WeaponGo.SetActive(false);
        Weapon weapon = WeaponGo.GetComponent<Weapon>();
        return WeaponGo;
    }

    public void PlayWeaponAnim(ToolType type, bool isPaused)
    {
        player.PlayWeayponAnimation(type, isPaused);
    }

    public void StopWeaponAnim(ToolType type, bool isPaused)
    {
        player.StopWeaponAnimation(type, isPaused);
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

    public void OnSelectedItem(GameObject item)
    {
        EquipTool(item.tag);
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
        //PlayerManager.SpawnMainPlayerEvent -= OnSpawnMainPlayer;
        SimpleScrollSnap.SelctedItemEvent -= OnSelectedItem;
    }
}

