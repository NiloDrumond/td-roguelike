using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
	public static Queue<TowerBehaviour> TowersToSpawn;
	public static Queue<Vector3Int> TowersToDestroy;
	public static List<TowerBehaviour> TowersInGame;
	public static TowerData[] TowerResources;
	public static int CurrentTowerSelected = 0;

	private static bool isInitialized;

	public static void Init()
	{
		if (!isInitialized)
		{
			TowersInGame = new List<TowerBehaviour>();
			TowerResources = Resources.LoadAll<TowerData>("Entities/Towers");
			TowersToSpawn = new Queue<TowerBehaviour>();
			TowersToDestroy = new Queue<Vector3Int>();
		}
		else
		{
			Debug.LogWarning("ENTITYSUMMONER: this class has already initialized");
		}
	}

	public static TowerBehaviour PlaceTower(Vector3Int position, Vector3 worldPosition)
	{
		if (CurrentTowerSelected >= 0)
		{
			TowerData towerData = TowerResources[CurrentTowerSelected];
			TowerBehaviour tower = new TowerBehaviour(towerData, position, worldPosition);
			TowersToSpawn.Enqueue(tower);
			return tower;
		}
		return null;
	}

	public static void SpawnTowers()
	{
		for (int i = 0; i < TowersToSpawn.Count; i++)
		{
			TowerBehaviour tower = TowersToSpawn.Dequeue();
			TowersInGame.Add(tower);
		}
	}

	public static void RemoveTower(Vector3Int position)
	{
		TowersToDestroy.Enqueue(position);
	}

	public static void RemoveTowers()
	{
		for (int i = 0; i < TowersToDestroy.Count; i++)
		{
			Vector3Int position = TowersToDestroy.Dequeue();
			int index = TowersInGame.FindIndex(t => t.Position.Equals(position));
			if(index >= 0)
			{
				TowersInGame.RemoveAt(index);
			}
		}
	}

	private void OnDrawGizmos()
	{
		if(TowersInGame != null)
		{
			foreach (TowerBehaviour t in TowersInGame.ToArray())
			{
				Gizmos.DrawWireSphere(t.WorldPosition, t.Range);
				if (t.Target != null)
				{
					Debug.DrawLine(t.WorldPosition, t.Target.transform.position);
				}
			}
		}
		
	}
}
