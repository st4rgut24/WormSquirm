using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum GateType
{
    Toll, // closes off segment from both ends
    Key,    // closes off segment from end
}

public class GateManager : Singleton<GateManager>
{
    public static event Action CreateGateEvent;

    public Transform GateParent;

    public GameObject TollPrefab;
    public GameObject GatePrefab;

	public List<Gate> GateList;

    private void Awake()
    {
		GateList = new List<Gate>();
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
		gate.SetCurSegment(segment);

		GateList.Add(gate);

        CreateGateEvent?.Invoke();

        return GateGo;
	}


	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
			
	}
}

