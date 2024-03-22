using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gate : Matter
{
    public enum State
    {
        Closed,
        Open
    }

    Jewel key;
    int priceToUnlock;
    State state;

    private void Awake()
    {
        state = State.Closed;

        // TODO: Create a Gatekeeper
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

    protected virtual bool IsKeyValid(Valuable valuable)
    {
        return key.GetType() == valuable.GetType();
    }

	protected void Open()
	{
        state = State.Open;
	}

    protected void Close()
    {
        state = State.Closed;
    }
}

