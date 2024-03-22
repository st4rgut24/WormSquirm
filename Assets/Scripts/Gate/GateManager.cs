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

    public static event Action CreateGateEvent;

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

    Dictionary<Jewel.Type, Jewel> KeyJewelPairs;

    private void OnEnable()
    {
		MainPlayer.CollectJewelEvent += OnKeyFound;
    }

    private void Awake()
    {
		GateList = new List<Gate>();

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
			returnSegments.RemoveAt(0); // remove the segment with key, because we don't want the gate to be added at the end of that segment
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

		for (int i=0;i< returnSegments.Count; i++)
		{
			if (i % gateSpacing == 0)
			{
                TypeProbability pickedTypeProbability = RandomUtils.PickTypeProbability(difficulty.GateTypeProbabilities);
                GateType randomGateType = (GateType)pickedTypeProbability.enumType;

				Create(returnSegments[i], randomGateType);
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
	private void InitGate(Gate gate, Segment segment)
	{
        gate.SetCurSegment(segment);

		Jewel gateKey;
		Vector3 keyPos;

        if (GateList.Count == 1)
        {
            StartGate = gate;
			gateKey = KeyJewelPairs[difficulty.FinalJewelType];
			keyPos = SegmentUtils.GetCollinearPoint(segment, difficulty.FinalKeyDist, difficulty.FinalKeyDirection);
        }
		else
		{
			gateKey = GetRandomKey();
			keyPos = SegmentUtils.GetPerpendicularPoint(segment, difficulty.AvgKeyDist, difficulty.KeyAngle);
        }

        Instantiate(gateKey.gameObject, keyPos, Quaternion.identity, KeyParent);
		gate.SetKey(gateKey);
    }

    private Jewel GetRandomKey()
    {
        // Get a random index
        Array jewelTypes = Enum.GetValues(typeof(Jewel.Type));
        int jewelIndex = UnityEngine.Random.Range(0, jewelTypes.Length);
        Jewel.Type randomJewelType = (Jewel.Type)jewelTypes.GetValue(jewelIndex);

        return KeyJewelPairs[randomJewelType];
    }

    public GameObject Create(Segment segment, GateType type) {
		GameObject GateGo;

		Vector3 GatePos = segment.endRingCenter;
		Quaternion GateRot = segment.GetRotation();


        switch (type) {
			case GateType.Toll:
				GateGo = Instantiate(TollPrefab, GatePos, GateRot, GateParent);
				break;
			case GateType.Key:
                GateGo = Instantiate(GatePrefab, GatePos, GateRot, GateParent);
                break;
			default:
				throw new System.Exception("Not a valid gate type: " + type);
		}

		Gate gate = GateGo.GetComponent<Gate>();
        GateList.Add(gate);
        InitGate(gate, segment);

        CreateGateEvent?.Invoke();

        return GateGo;
	}

    private void OnDisable()
    {
        MainPlayer.CollectJewelEvent -= OnKeyFound;
    }
}

