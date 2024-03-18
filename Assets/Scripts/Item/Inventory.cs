using System;
using System.Collections.Generic;

public class Inventory
{
	int fiat;
	List<Valuable> valuables;	

	public Inventory()
	{
		fiat = 0;
		valuables = new List<Valuable>();
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

	public void AddValuable(Valuable valuable)
	{
		valuables.Add(valuable);
	}

	/// <summary>
	/// Melts a valuable and turns it into cash
	/// </summary>
	/// <param name="valuable">the item to liquidate</param>
	public void Liquidate(Valuable valuable)
	{
		valuables.Remove(valuable);
		fiat += valuable.value;
	}
}

