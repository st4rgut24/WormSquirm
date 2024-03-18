using UnityEngine;
using System.Collections;

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

