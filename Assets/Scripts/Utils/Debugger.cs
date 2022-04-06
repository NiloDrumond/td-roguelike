using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger
{
    public static void Log<T>(T[] arr) 
	{
		for (int i = 0; i < arr.Length; i++)
		{
			Debug.Log(i + " " + arr[i]);
		}
	}

	public static void Log<T>(T list) where T : List<T>
	{
		foreach(var item in list)
		{
			Debug.Log(item);
		}
	}
}
