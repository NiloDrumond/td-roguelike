using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
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
			TowersInGame.Add(tower);
			return tower;
		}
		return null;
	}


	public static TowerBehaviour RemoveTower(Vector3Int position)
	{
		TowerBehaviour tower = null;
		foreach(TowerBehaviour t in TowersInGame.ToArray())
		{
			if(t.Position == position)
			{
				TowersInGame.Remove(t);
				tower = t;
			}
		}
		return tower;
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
