using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollSnapCanvas : MonoBehaviour
{
	// Use this for initialization
	void Start()
	{
		GetComponent<Canvas>().worldCamera = transform.parent.GetComponent<Camera>();
    }

	// Update is called once per frame
	void Update()
	{
			
	}
}

