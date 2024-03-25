using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using static CharacterAnimator;

public enum GateType
{
    Toll, // closes off segment from both ends
    Key,    // closes off segment from end
}

public class GateManager : Singleton<GateManager>
{
	GateManagerDifficulty difficulty;

	HashSet<Jewel.Type> KeySet; // Set of keys used to open gates, used to ensure uniqueness of generated keys

    public static event Action CreateGateEvent;
    public static event Action<GameObject> DestroyGateEvent;

    public static event Action<GameObject> CreateKeyEvent;
    public static event Action<GameObject> DestroyKeyEvent;

    public Transform GateParent;
    public Transform KeyParent;

    public GameObject TollPrefab;
    public GameObject GatePrefab;

	public List<Gate> GateList;
	public Gate StartGate;

	public Jewel AquamarineKey;
	public Jewel EmeraldKey;
	public Jewel GoldenStarKey;
	public Jewel MoonStoneKey;
	public Jewel ObsidianKey;
	public Jewel PinkSaphireKey;
	public Jewel RubyKey;

    //public Dictionary<string, GameObject> KeyDict; // <enum of the Jewel/key, Key GameObject>. Assuming the keys that are created are unique
    Dictionary<Jewel.Type, Jewel> KeyJewelPairs;

    private void OnEnable()
    {
		Jewel.CollectJewelEvent += OnKeyFound;
    }

    private void Awake()
    {
		GateList = new List<Gate>();
		KeySet = new HashSet<Jewel.Type>();

        KeyJewelPairs = new Dictionary<Jewel.Type, Jewel>
		{
			{ Jewel.Type.Aquamarine, AquamarineKey },
            { Jewel.Type.Emerald, EmeraldKey },
            { Jewel.Type.GoldenStar, GoldenStarKey },
            { Jewel.Type.MoonStone, MoonStoneKey },
            { Jewel.Type.Obsidian, ObsidianKey },
            { Jewel.Type.PinkSaphire, PinkSaphireKey },
            { Jewel.Type.Ruby, RubyKey }
        };

        //KeyDict = new Dictionary<string, GameObject>();
    }

    /// <summary>
    /// If the start gate jewel is found, initialize the rest of the gates
    /// </summary>
    /// <param name="jewel">a found jewel</param>
    /// <param name="segment">segment containing the jewel</param>
    public void OnKeyFound(Jewel jewel, Segment segment)
	{
		if (GateList.Count == 1 && StartGate.GetKeyType() == jewel.type) // check if this is the starting gate's jewel
		{
			// 1. find the route back to the start
			Segment finalGateSegment = SegmentUtils.GetNextSegment(StartGate.curSegment);
			List<Segment> returnSegments = SearchUtils.dfsConnectSegments(finalGateSegment, segment);
			//because it would be in the wrong direction
			initIntermediaryGates(returnSegments);
        }
	}

    /// <summary>
    /// Create the gates en route to the starting ate
    /// </summary>
    /// <param name="returnSegments">ordered segments on the return journey</param>
    public void initIntermediaryGates(List<Segment> returnSegments)
	{
		int totalGates = difficulty.GateCount;
        int gateSpacing = returnSegments.Count / totalGates; // units are in segments

		if (gateSpacing == 0)
		{
			Debug.LogWarning("The route is not long enough to add gates to");
			return;
		}

		// start at the first gate following the gate with the gate the key was found in
		// aka the first tunnel behind the player
		for (int i=1; i< returnSegments.Count; i++)
		{
			if (i % gateSpacing == 0)
			{
                TypeProbability pickedTypeProbability = RandomUtils.PickTypeProbability(difficulty.GateTypeProbabilities);
                GateType randomGateType = (GateType)pickedTypeProbability.enumType;
				Segment prevSegment = returnSegments[i - 1];
				Create(returnSegments[i],prevSegment, randomGateType);
            }
		}
    }

	public void SetParams(GateManagerDifficulty difficulty)
	{
		this.difficulty = difficulty;
    }

    public Gate GetNewestGate()
	{
		if (GateList.Count == 0)
		{
			return null;
		}
		else
		{
			return GateList[GateList.Count - 1];
        }
    }

	/// <summary>
	/// Initialize the gate with a key
	/// </summary>
	/// <param name="gate">gate</param>
	/// <param name="segment">segment the gate is in</param>
	private void InitGate(Gate gate, Segment gateSegment, Segment keySegment)
	{
        gate.SetCurSegment(gateSegment);

		Jewel gateKeyPrefab;
		Key key;

        if (GateList.Count == 1)
        {
            StartGate = gate;
			gateKeyPrefab = KeyJewelPairs[difficulty.FinalJewelType];
			key = new Key(keySegment, difficulty.FinalKeyDist, difficulty.FinalKeyDirection);
        }
		else
		{
			gateKeyPrefab = GetRandomUniqueKey();
			key = new Key(keySegment, difficulty.AvgKeyDist, difficulty.KeyMinAngle, difficulty.KeyMaxAngle);
        }

		InitKey(gateKeyPrefab, key, gate);
    }

	/// <summary>
	/// Create a key and initialize everything that depends on it
	/// </summary>
	/// <param name="gateKeyPrefab">the prefab of the key</param>
	/// <param name="key">contains key metadata like position</param>
	/// <param name="gate">Gate the key belongs to</param>
	private void InitKey(Jewel gateKeyPrefab, Key key, Gate gate)
	{
        GameObject keyGo = Instantiate(gateKeyPrefab.gameObject, key.position, Quaternion.identity, KeyParent);

        GameObject KeyGoInScene = SceneItemManager.Instance.AddSceneObject(keyGo);
        Jewel KeyInScene = KeyGoInScene.GetComponent<Jewel>();
        KeySet.Add(KeyInScene.type);

        CreateKeyEvent?.Invoke(keyGo);

        gate.SetKey(gateKeyPrefab);
    }

	/// <summary>
	/// Pick a key for a gate randomly. Ensure there are no duplicate keys created
	/// </summary>
	/// <returns>A key that unlocks a gate</returns>
    private Jewel GetRandomUniqueKey()
    {
        Array jewelTypes = Enum.GetValues(typeof(Jewel.Type));

		if (KeySet.Count == jewelTypes.Length)
		{
			throw new Exception("There are no more keys left to choose from");
		}

		Jewel.Type randomJewelType;
        do
        {
            int jewelIndex = UnityEngine.Random.Range(0, jewelTypes.Length);
            randomJewelType = (Jewel.Type)jewelTypes.GetValue(jewelIndex);
        } while (KeySet.Contains(randomJewelType));


        return KeyJewelPairs[randomJewelType];
    }

	/// <summary>
	/// Destroy a gate after it has been opened
	/// </summary>
	/// <param name="gate">the gate to remove</param>
	public void Destroy(Gate gate)
	{
		Jewel keyPrefab = gate.GetKey();
        GameObject keyGo = SceneItemManager.Instance.GetSceneObject(keyPrefab.tag);

        GateList.Remove(gate);
		KeySet.Remove(keyPrefab.type);

        DestroyKeyEvent?.Invoke(keyGo);
        DestroyGateEvent?.Invoke(gate.gameObject);

        GameObject.Destroy(keyGo);
        GameObject.Destroy(gate.gameObject);
    }

    public GameObject Create(Segment segment, Segment prevSegment, GateType type) {
		GameObject GateGo;

		Vector3 GatePos = segment.endRingCenter;

        switch (type) {
			case GateType.Toll:
				GateGo = Instantiate(TollPrefab, GatePos, segment.rotation, GateParent);
				break;
			case GateType.Key:
                GateGo = Instantiate(GatePrefab, GatePos, segment.rotation, GateParent);
                break;
			default:
				throw new System.Exception("Not a valid gate type: " + type);
		}

		Gate gate = GateGo.GetComponent<Gate>();
        GateList.Add(gate);
        InitGate(gate, segment, prevSegment);

        CreateGateEvent?.Invoke();

        return GateGo;
	}

    private void OnDisable()
    {
        Jewel.CollectJewelEvent -= OnKeyFound;
    }
}

