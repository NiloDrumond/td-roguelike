using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GeneratorManager: MonoBehaviour {
    private static Queue<SupplySpawnData> GeneratorToSpawn;
    private static Queue<Vector3> GeneratorToDestroy;
    public static List<GeneratorBehaviour> GeneratorInGame;
    public static Dictionary<int, GameObject> GeneratorPrefabs;
    public static int GeneratorToBuild = 0;

	private static bool isInitialized = false;
	private static GameObject generatorsParent;

    public static void Init()
	{
		if (!isInitialized)
		{
			GeneratorInGame = new List<GeneratorBehaviour>();
			GeneratorData[] generatorResources = Resources.LoadAll<GeneratorData>("Entities/Generators");
			GeneratorPrefabs = new Dictionary<int, GameObject>();
			
			for (int i = 0; i < generatorResources.Length; i++)
			{
				GeneratorPrefabs.Add(generatorResources[i].GeneratorID, generatorResources[i].GeneratorPrefab);
			}

			GeneratorToSpawn = new Queue<SupplySpawnData>();
			GeneratorToDestroy = new Queue<Vector3>();


			generatorsParent = GameObject.Find("Generators");

			isInitialized = true;
		}
		else
		{
			Debug.LogWarning("SUPPLYMANAGER: this class has already initialized");
		}
	}


	public static void SelectSupplyToBuild(int index)
	{
		GeneratorToBuild = index;
	}

	public static bool PlaceSupply(Vector3 worldPosition)
	{
		if (GeneratorToBuild >= 0)
		{
			SupplySpawnData data = new SupplySpawnData() { ID = GeneratorToBuild, Position = worldPosition };
			GeneratorToSpawn.Enqueue(data);
			return true;
		}
		return false;
	}

	public static void SpawnGenerators()
	{
		for (int i = 0; i < GeneratorToSpawn.Count; i++)
		{
			SupplySpawnData data = GeneratorToSpawn.Dequeue();
			GameObject supply = Instantiate(GeneratorPrefabs[data.ID], data.Position, Quaternion.identity, generatorsParent.transform);
			supply.transform.position = new Vector3(supply.transform.position.x, supply.transform.position.y, supply.transform.position.z + data.Position.z);
			GeneratorBehaviour supplyBehaviour = supply.GetComponent<GeneratorBehaviour>();
			supplyBehaviour.Init();
			GeneratorInGame.Add(supplyBehaviour);
		}
	}

	public static void RemoveSupply(Vector3 position)
	{
		GeneratorToDestroy.Enqueue(position);
	}

	public static void RemoveGenerators()
	{
		for (int i = 0; i < GeneratorToDestroy.Count; i++)
		{
			Vector3 position = GeneratorToDestroy.Dequeue();
			int index = GeneratorInGame.FindIndex(t => {
				return t.transform.position.x == position.x && t.transform.position.y == position.y;
			});
			if(index >= 0)
			{
				GameObject obj = GeneratorInGame[index].gameObject;
				GeneratorInGame.RemoveAt(index);
				Destroy(obj);
			}
		}
	}
}

public struct SupplySpawnData
{
	public Vector3 Position;
	public int ID;
}