using System;
public enum ValueType
{
    Jewel
}

public class Valuable
{
	ValueType type;

	public int value;

	public Valuable(int value, ValueType type)
	{
		this.value = value;
		this.type = type;
	}

	public ValueType GetType()
	{
		return type;
	}
}

