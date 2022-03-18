using System.Collections.Generic;
using UnityEngine;

public class EntitySummoner : MonoBehaviour
{
	public static List<Enemy> EnemiesInGame;
	public static List<Transform> EnemiesInGameTransform;
    public static Dictionary<int, GameObject> EnemyPrefabs;
    public static Dictionary<int, Queue<Enemy>> EnemyObjectPools;
	
	private static GameObject enemiesParent;
	private static bool isInitialized;

	public static void Init()
	{
		if(!isInitialized)
		{
			EnemyPrefabs = new Dictionary<int, GameObject>();
			EnemyObjectPools = new Dictionary<int, Queue<Enemy>>();
			EnemiesInGame = new List<Enemy>();
			EnemiesInGameTransform = new List<Transform>();
			enemiesParent = GameObject.Find("Enemies");

			EnemySummon[] EnemyResources = Resources.LoadAll<EnemySummon>("Entities/Enemies");
			foreach (EnemySummon enemy in EnemyResources)
			{
				EnemyPrefabs.Add(enemy.EnemyID, enemy.EnemyPrefab);
				EnemyObjectPools.Add(enemy.EnemyID, new Queue<Enemy>());
			}
		}
		else
		{
			Debug.LogWarning("ENTITYSUMMONER: this class has already initialized");
		}
	}

	public static Enemy SummonEnemy(int enemyId)
	{
		Enemy summonedEnemy = null;

		if (EnemyPrefabs.ContainsKey(enemyId))
		{
			Queue<Enemy> referencedQueue = EnemyObjectPools[enemyId];
			if(referencedQueue.Count > 0)
			{
				summonedEnemy = referencedQueue.Dequeue();
				summonedEnemy.Init();
				summonedEnemy.gameObject.SetActive(true);
			} else
			{
				GameObject newEnemy = Instantiate(EnemyPrefabs[enemyId], GameLoopManager.WaypointPositions[0], Quaternion.identity, enemiesParent.transform);
				summonedEnemy = newEnemy.GetComponent<Enemy>();
				summonedEnemy.Init();
			}
		} else
		{
			Debug.LogWarning($"ENTITYSUMMONER: enemy with id {enemyId} not found");
			return null;
		}
		EnemiesInGame.Add(summonedEnemy);
		EnemiesInGameTransform.Add(summonedEnemy.transform);
		summonedEnemy.ID = enemyId;
		return summonedEnemy;
	}

	public static void RemoveEnemy(Enemy enemy)
	{
		EnemyObjectPools[enemy.ID].Enqueue(enemy);
		enemy.gameObject.SetActive(false);
		EnemiesInGame.Remove(enemy);
		EnemiesInGameTransform.Remove(enemy.transform);
	}
}
