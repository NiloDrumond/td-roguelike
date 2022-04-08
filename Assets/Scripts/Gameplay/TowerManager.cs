using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
	private static Queue<TowerSpawnData> TowersToSpawn;
	private static Queue<Vector3> TowersToDestroy;
	public static List<TowerBehaviour> TowersInGame;
	public static Dictionary<int, GameObject> TowerPrefabs;
	public static int TowerToBuild = 0;

	private static bool isInitialized = false;
	private static GameObject towersParent;


	public static void Init()
	{
		if (!isInitialized)
		{
			TowersInGame = new List<TowerBehaviour>();
			TowerData[] towerResources = Resources.LoadAll<TowerData>("Entities/Towers");
			TowerPrefabs = new Dictionary<int, GameObject>();

			for (int i = 0; i < towerResources.Length; i++)
			{
				TowerPrefabs.Add(towerResources[i].TowerID, towerResources[i].TowerPrefab);
			}

			TowersToSpawn = new Queue<TowerSpawnData>();
			TowersToDestroy = new Queue<Vector3>();


			towersParent = GameObject.Find("Towers");

			isInitialized = true;
		}
		else
		{
			Debug.LogWarning("ENTITYSUMMONER: this class has already initialized");
		}
	}


	public static void SelectTowerToBuild(int index)
	{
		TowerToBuild = index;
	}

	public static bool PlaceTower(Vector3 worldPosition)
	{
		if (TowerToBuild >= 0)
		{
			TowerSpawnData data = new TowerSpawnData() { ID = TowerToBuild, Position = worldPosition };
			TowersToSpawn.Enqueue(data);
			return true;
		}
		return false;
	}

	public static void SpawnTowers()
	{
		for (int i = 0; i < TowersToSpawn.Count; i++)
		{
			TowerSpawnData data = TowersToSpawn.Dequeue();
			GameObject tower = Instantiate(TowerPrefabs[data.ID], data.Position, Quaternion.identity, towersParent.transform);
			tower.transform.position = new Vector3(tower.transform.position.x, tower.transform.position.y, tower.transform.position.z + data.Position.z);
			TowerBehaviour towerBehaviour = tower.GetComponent<TowerBehaviour>();
			TowersInGame.Add(towerBehaviour);
		}
	}

	public static void RemoveTower(Vector3 position)
	{
		TowersToDestroy.Enqueue(position);
	}

	public static void RemoveTowers()
	{
		for (int i = 0; i < TowersToDestroy.Count; i++)
		{
			Vector3 position = TowersToDestroy.Dequeue();
			int index = TowersInGame.FindIndex(t => {
				return t.transform.position.x == position.x && t.transform.position.y == position.y;
			});
			if(index >= 0)
			{
				GameObject obj = TowersInGame[index].gameObject;
				TowersInGame.RemoveAt(index);
				Destroy(obj);
			}
		}
	}
}

public struct TowerSpawnData
{
	public Vector3 Position;
	public int ID;
}