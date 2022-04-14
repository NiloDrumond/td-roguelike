using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public enum Supply
{
	Mineral
}

[Serializable]
public class Supplies : Dictionary<Supply, int>
{
	public Supplies() { }

	public Supplies(int mineralsCost)
	{
		this.Add(Supply.Mineral, mineralsCost);
	}

	public static Supplies Clone(Supplies a)
	{
		Supplies b = new Supplies();
		foreach (var pair in a)
		{
			b.Add(pair.Key, pair.Value);
		}
		return b;
	}

	public void Log()
	{
		bool noPairs = true;
		foreach (var pair in this)
		{
			noPairs = false;
			Debug.Log($"{pair.Key}: {pair.Value}");
		}
		if (noPairs)
		{
			Debug.Log("No pair inside Supplies");
		}
	}

	public bool CanSubtract(Supplies b)
	{
		foreach (var pair in b)
		{
			if (!this.ContainsKey(pair.Key)) return false;
			if (this[pair.Key] < pair.Value) return false;
		}
		return true;
	}

	public static Supplies operator -(Supplies a, Supplies b)
	{
		Supplies c = Clone(a);
		foreach(var pair in b)
		{
			if(b.ContainsKey(pair.Key))
			{
				c[pair.Key] =  c[pair.Key] - pair.Value;
			}
		}
		return c;
	}

	public static Supplies operator +(Supplies a, Supplies b)
	{
		Supplies c = Clone(a);
		foreach (var pair in b)
		{
			if (c.ContainsKey(pair.Key))
			{
				c[pair.Key] += pair.Value;
			}
			else
			{
				c.Add(pair.Key, pair.Value);
			}
		}
		return c;
	}

}