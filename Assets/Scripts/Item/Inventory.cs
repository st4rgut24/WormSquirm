using System;
using System.Collections.Generic;
using System.Linq;
using DanielLochner.Assets.SimpleScrollSnap;
using UnityEditor.UIElements;
using UnityEngine;

/// <summary>
/// Accounting system
/// </summary>
public class Inventory : MonoBehaviour
{
    public GameObject ValuablesSliderGo;
    public ValuablesSlider ValuablesSlider;

    public GameObject AquamarineUIPrefab;
    public GameObject EmeraldUIPrefab;
    public GameObject GoldenStarUIPrefab;
    public GameObject MoonStoneUIPrefab;
    public GameObject ObsidianUIPrefab;
    public GameObject PinkSaphireUIPrefab;
    public GameObject RubyUIPrefab;

    private MainPlayer mainPlayer;

    int fiat;
	List<Equipment> collectibles;

	Dictionary<int, GameObject> ValuablesUIPrefabs;

    private Equipment activeEquipment; // equipment that is being used by player

    private void Awake()
    {
        ValuablesUIPrefabs = new Dictionary<int, GameObject>();

        SimpleScrollSnap valuablesSnap = ValuablesSliderGo.GetComponent<ValuablesSnap>();
        fiat = 0;
        collectibles = new List<Equipment>();
        InitValuablesPrefabDict();

        ValuablesSlider = new ValuablesSlider(valuablesSnap, ValuablesUIPrefabs);
    }

    private void OnEnable()
    {
        Detector.DetectDistanceEvent += OnDistanceToItemReceived;
    }

    public void InitValuablesPrefabDict()
    {
		ValuablesUIPrefabs[(int)(Jewel.Type.Aquamarine)] = AquamarineUIPrefab;
		ValuablesUIPrefabs[(int)(Jewel.Type.Emerald)] = EmeraldUIPrefab;
        ValuablesUIPrefabs[(int)(Jewel.Type.GoldenStar)] = GoldenStarUIPrefab;
        ValuablesUIPrefabs[(int)(Jewel.Type.MoonStone)] = MoonStoneUIPrefab;
        ValuablesUIPrefabs[(int)(Jewel.Type.Obsidian)] = ObsidianUIPrefab;
        ValuablesUIPrefabs[(int)(Jewel.Type.PinkSaphire)] = PinkSaphireUIPrefab;
        ValuablesUIPrefabs[(int)(Jewel.Type.Ruby)] = RubyUIPrefab;
    }

    public int GetFiat()
	{
		return fiat;
	}

	/// <summary>
	/// Pay for something
	/// </summary>
	/// <param name="price">price of the thing to buy</param>
	/// <returns>true if enough money to pay</returns>
	public bool Pay(int price)
	{
		if (price <= fiat)
		{
			fiat -= price;
			return true;
		}
		else {
			return false;
		}
	}

    public void SetMainPlayer(MainPlayer player)
    {
        this.mainPlayer = player;
    }

	public void Add(Equipment item)
	{
		collectibles.Add(item);
	}

    /// <summary>
    /// Check whether a equippable item is in the player's inventory
    /// </summary>
    /// <param name="item">an item</param>
    /// <returns>true if player has collected the item</returns>
    public bool InInventory(GameObject item)
    {
        Equipment equipment = item.GetComponent<Equipment>();

        if (equipment == null) 
        {
            return false;
        }
        else
        {
            return collectibles.Contains(equipment);
        }
    }

	/// <summary>
	/// Melts a valuable and turns it into cash
	/// </summary>
	/// <param name="valuable">the item to liquidate</param>
	public void Liquidate(Equipment valuable)
	{
		collectibles.Remove(valuable);
		fiat += valuable.value;
	}

    public void OnKeyFound(Jewel key, Segment segment)
    {
        Add(key);
        ValuablesSlider.SetSlideText(key.tag, "Found");
        Equip(key);
    }

    public void OnKeyCreated(GameObject keyGo)
    {
		Jewel key = keyGo.GetComponent<Jewel>();
		ValuablesSlider.OnKeyCreated(key);
    }

    public void OnKeyDestroyed(GameObject keyGo)
    {
        Jewel key = keyGo.GetComponent<Jewel>();
        ValuablesSlider.OnKeyDestroyed(key);
    }

    /// <summary>
    /// Handle notification regarding an item's proximity changing using the items UI 
    /// </summary>
    /// <param name="itemTag">tag of the item</param>
    /// <param name="distance">distance to the ite</param>
    public void OnDistanceToItemReceived(string itemTag, float distance)
    {
        string distText = distance.ToString() + " " + Consts.Meters;
        ValuablesSlider.SetSlideText(itemTag, distText);
    }

    /// <summary>
    /// Item is the UI Slider item, not the item that is collected in gae
    /// </summary>
    /// <param name="item">UI Slider item</param>
    public void OnSelectedItem(GameObject item)
	{
        if (TransformUtils.IsTransformMatchTags(item.transform, Consts.EquipmentTags) )
        {

            StoreActiveEquipment();

            GameObject sceneItem = SceneItemManager.Instance.GetSceneObject(item.tag);
            if (InInventory(sceneItem))
            {
                // equip the new item
                Equipment equipment = sceneItem.GetComponent<Equipment>();
                Equip(equipment);
            }
        }
        else
        {
            // some non-equipment item was selected like a weapon. Not the inventory's problem.
        }
    }

    // put back the current equipped item
    private void StoreActiveEquipment()
    {
        if (activeEquipment != null)
        {
            activeEquipment.PutBack();
            activeEquipment = null;
        }
    }

    /// <summary>
    /// Equip the player
    /// </summary>
    /// <param name="equipment">the thing to equip</param>
    private void Equip(Equipment equipment)
    {
        StoreActiveEquipment();

        mainPlayer.EquipValuable(equipment.gameObject);
        equipment.TakeOut();
        activeEquipment = equipment;
    }

    private void OnDisable()
    {
        Detector.DetectDistanceEvent -= OnDistanceToItemReceived;
    }
}

