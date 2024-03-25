using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gate : Matter
{
    public enum State
    {
        Closed,
        Opening,
        Open
    }

    Jewel key;
    int priceToUnlock;
    State state;

    private Vector3 openPosition;
    private Vector3 closedPosition;

    private void Awake()
    {
        state = State.Closed;

        closedPosition = transform.position;
        openPosition = transform.position + transform.up * Consts.OpenDist;
    }

    private void Update()
    {
        if (IsOpening())
        {
            transform.position = Vector3.Lerp(transform.position, openPosition, Consts.OpenSpeed * Time.deltaTime);
        }
        else if (state == State.Opening)
        {
            // finished opening
            state = State.Open;
            GateManager.Instance.Destroy(this);
        }
    }

    private bool IsOpening()
    {
        return state == State.Opening && !TransformUtils.ReachedDestination(transform.position, openPosition);
    }

    public Jewel.Type GetKeyType()
    {
        return key.type;
    }

    public Jewel GetKey()
    {
        return key;
    }

    /// <summary>
    /// Create a key that unlocks this gate
    /// </summary>
    /// <param name="direction">direction in which key can be found</param>
    /// <param name="distance">distance along the direction to find the key</param>
    public void CreateKey(Vector3 direction, Vector3 distance)
    {

    }

    public virtual void SetKey(Jewel key)
    {
        this.key = key;
    }

    public void Unlock(Jewel jewel)
    {
        if (IsKeyValid(jewel))
        {
            Open();
        }
        else
        {
            Debug.LogWarning("This valuable is not sufficent to open the gate");
        }
    }

    /// <summary>
    /// player will need to equip jewel in hand and touch the gate to open it
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (TransformUtils.IsTransformMatchTags(other.transform, Consts.KeyTags))
        {
            Jewel key = other.GetComponent<Jewel>();
            Unlock(key);
        }
    }

    protected virtual bool IsKeyValid(Jewel foundKey)
    {
        return key.type == foundKey.type;
    }

	protected void Open()
	{
        state = State.Opening;
	}

    protected void Close()
    {
        state = State.Closed;
    }
}

