using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
	public static List<Enemy> EnemiesInGame;
	public static List<Transform> EnemiesInGameTransform;

	public static Dictionary<Transform, Enemy> EnemyTransformMap;
    public static Dictionary<int, GameObject> EnemyPrefabs;
    public static Dictionary<int, Queue<Enemy>> EnemyObjectPools;
	
	private static GameObject enemiesParent;
	private static bool isInitialized = false;

	public static void Init()
	{
		if(!isInitialized)
		{
			EnemyPrefabs = new Dictionary<int, GameObject>();
			EnemyObjectPools = new Dictionary<int, Queue<Enemy>>();
			EnemyTransformMap = new Dictionary<Transform, Enemy>();
			EnemiesInGame = new List<Enemy>();
			EnemiesInGameTransform = new List<Transform>();
			enemiesParent = GameObject.Find("Enemies");

			EnemySummon[] EnemyResources = Resources.LoadAll<EnemySummon>("Entities/Enemies");
			foreach (EnemySummon enemy in EnemyResources)
			{
				EnemyPrefabs.Add(enemy.EnemyID, enemy.EnemyPrefab);
				EnemyObjectPools.Add(enemy.EnemyID, new Queue<Enemy>());
			}
			isInitialized = true;
		}
		else
		{
			Debug.LogWarning("ENTITYSUMMONER: this class has already initialized");
		}
	}

	public static Enemy SummonEnemy(EnemyCreateData data)
	{
		Enemy summonedEnemy = null;

		if (EnemyPrefabs.ContainsKey(data.EnemyId))
		{
			Queue<Enemy> referencedQueue = EnemyObjectPools[data.EnemyId];
			if(referencedQueue.Count > 0)
			{
				summonedEnemy = referencedQueue.Dequeue();
				summonedEnemy.Init(data.PathIndex);
				summonedEnemy.gameObject.SetActive(true);
			} else
			{
				GameObject newEnemy = Instantiate(EnemyPrefabs[data.EnemyId], PathsManager.WaypointPositions[0][0], Quaternion.identity, enemiesParent.transform);
				summonedEnemy = newEnemy.GetComponent<Enemy>();
				summonedEnemy.Init(data.PathIndex);
			}
		} else
		{
			Debug.LogWarning($"ENTITYSUMMONER: enemy with id {data.EnemyId} not found");
			return null;
		}
		
		if(!EnemiesInGame.Contains(summonedEnemy)) EnemiesInGame.Add(summonedEnemy);
		if(!EnemiesInGameTransform.Contains(summonedEnemy.transform))  EnemiesInGameTransform.Add(summonedEnemy.transform);
		if (!EnemyTransformMap.ContainsKey(summonedEnemy.transform)) EnemyTransformMap.Add(summonedEnemy.transform, summonedEnemy);
		summonedEnemy.ID = data.EnemyId;
		return summonedEnemy;
	}

	public static void RemoveEnemy(Enemy enemy)
	{
		EnemyObjectPools[enemy.ID].Enqueue(enemy);
		enemy.gameObject.SetActive(false);
		EnemiesInGame.Remove(enemy);
		EnemyTransformMap.Remove(enemy.transform);
		EnemiesInGameTransform.Remove(enemy.transform);
	}
}

public struct EnemyCreateData 
{
	public int EnemyId;
	public int PathIndex;

	public EnemyCreateData(int enemyId, int pathIndex)
	{
		EnemyId = enemyId;
		PathIndex = pathIndex;
	}
};