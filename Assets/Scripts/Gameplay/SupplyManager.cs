using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyManager: MonoBehaviour {
    private static Queue<SupplySpawnData> SuppliesToSpawn;
    private static Queue<Vector3> SuppliesToDestroy;
    public static List<Supply> SuppliesInGame;
    public static Dictionary<int, GameObject> SupplyPrefabs;
    public static int SupplyToBuild = 0;

	private static bool isInitialized = false;
	private static GameObject suppliesParent;

    public static void Init()
	{
		if (!isInitialized)
		{
			SuppliesInGame = new List<Supply>();
			SupplyData[] supplyResources = Resources.LoadAll<SupplyData>("Entities/Supplies");
			SupplyPrefabs = new Dictionary<int, GameObject>();
			
			for (int i = 0; i < supplyResources.Length; i++)
			{
				SupplyPrefabs.Add(supplyResources[i].SupplyID, supplyResources[i].SupplyPrefab);
			}
			/*
			foreach (SupplyData enemy in supplyResources)
			{
				SupplyPrefabs.Add(enemy.SupplyID, enemy.SupplyPrefab);
			}
			*/

			SuppliesToSpawn = new Queue<SupplySpawnData>();
			SuppliesToDestroy = new Queue<Vector3>();


			suppliesParent = GameObject.Find("Supplies");

			isInitialized = true;
		}
		else
		{
			Debug.LogWarning("ENTITYSUMMONER: this class has already initialized");
		}
	}


	public static void SelectSupplyToBuild(int index)
	{
		SupplyToBuild = index;
	}

	public static bool PlaceSupply(Vector3 worldPosition)
	{
		if (SupplyToBuild >= 0)
		{
			SupplySpawnData data = new SupplySpawnData() { ID = SupplyToBuild, Position = worldPosition };
			SuppliesToSpawn.Enqueue(data);
			return true;
		}
		return false;
	}

	public static void SpawnSupplies()
	{
		for (int i = 0; i < SuppliesToSpawn.Count; i++)
		{
			SupplySpawnData data = SuppliesToSpawn.Dequeue();
			GameObject supply = Instantiate(SupplyPrefabs[data.ID], data.Position, Quaternion.identity, suppliesParent.transform);
			supply.transform.position = new Vector3(supply.transform.position.x, supply.transform.position.y, supply.transform.position.z + data.Position.z);
			Supply supplyBehaviour = supply.GetComponent<Supply>();
			SuppliesInGame.Add(supplyBehaviour);
		}
	}

	public static void RemoveSupply(Vector3 position)
	{
		SuppliesToDestroy.Enqueue(position);
	}

	public static void RemoveSupplies()
	{
		for (int i = 0; i < SuppliesToDestroy.Count; i++)
		{
			Vector3 position = SuppliesToDestroy.Dequeue();
			int index = SuppliesInGame.FindIndex(t => {
				return t.transform.position.x == position.x && t.transform.position.y == position.y;
			});
			if(index >= 0)
			{
				GameObject obj = SuppliesInGame[index].gameObject;
				SuppliesInGame.RemoveAt(index);
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