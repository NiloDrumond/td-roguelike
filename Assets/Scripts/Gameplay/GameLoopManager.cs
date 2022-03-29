using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class GameLoopManager : MonoBehaviour
{
    public static Vector3[] WaypointPositions;
    public static float[] WaypointDistances;
    public static float[] WaypointDistancesToEnd;
    private static PathController pathController;

    private static Queue<EnemyDamageData> damageData;

    private static Queue<Enemy> enemiesToRemove;
    private static Queue<int> enemyIDsToSummon;

    public static bool LoopShouldEnd;
    // Start is called before the first frame update
    void Start()
    {

        damageData = new Queue<EnemyDamageData>();
        enemyIDsToSummon = new Queue<int>();
        enemiesToRemove = new Queue<Enemy>();

        PlayerManager.Init();
        EntityManager.Init();
        TowerManager.Init();
        InputManager.Init();

        pathController = GameObject.Find("Grid/Path").GetComponent<PathController>();

        WaypointPositions = pathController.GetWorldArray();
        WaypointDistances = new float[Mathf.Max(0, WaypointPositions.Length - 1)];
		for (int i = 0; i < WaypointDistances.Length; i++)
		{
            WaypointDistances[i] = Vector3.Distance(WaypointPositions[i], WaypointPositions[i + 1]); 
		}

        WaypointDistancesToEnd = new float[WaypointPositions.Length];
        for (int i = 0; i < WaypointDistances.Length; i++)
		{
            float distance = 0;
            for(int j = i; j < WaypointDistances.Length; j++)
			{
                distance += WaypointDistances[j];
            }
            WaypointDistancesToEnd[i] = distance;
        }



        StartCoroutine(GameLoop());
        InvokeRepeating("SummonTest", 0f, 1f);
    }

    void SummonTest()
	{
        EnqueueEnemyIDToSummon(1);
    }


    // Update is called once per frame
   IEnumerator GameLoop()
	{
        while(LoopShouldEnd == false)
		{
            // Spawn Enemies
            
            if(enemyIDsToSummon.Count > 0)
			{
                for(int i = 0; i < enemyIDsToSummon.Count; i++)
				{
                    EntityManager.SummonEnemy(enemyIDsToSummon.Dequeue());
				}
			}

            // Spawn Towers

            TowerManager.SpawnTowers();

            // Move Enemies

            NativeArray<Vector3> waypointsToUse = new NativeArray<Vector3>(WaypointPositions, Allocator.TempJob);
            NativeArray<int> waypointIndices = new NativeArray<int>(EntityManager.EnemiesInGame.Count, Allocator.TempJob);
            NativeArray<float> enemySpeeds = new NativeArray<float>(EntityManager.EnemiesInGame.Count, Allocator.TempJob);
            TransformAccessArray enemyAccess = new TransformAccessArray(EntityManager.EnemiesInGameTransform.ToArray(), 2);

			for (int i = 0; i < EntityManager.EnemiesInGame.Count; i++)
			{
                enemySpeeds[i] = EntityManager.EnemiesInGame[i].Speed;
                waypointIndices[i] = EntityManager.EnemiesInGame[i].WaypointIndex;
			}

            MoveEnemiesJob moveJob = new MoveEnemiesJob {
                enemySpeeds = enemySpeeds,
                WaypointIndex = waypointIndices,
                WaypointPositions = waypointsToUse,
                deltaTime = Time.deltaTime
            };

            JobHandle moveJobHandle = moveJob.Schedule(enemyAccess);
            moveJobHandle.Complete();

			for (int i = 0; i < EntityManager.EnemiesInGame.Count; i++)
			{
                EntityManager.EnemiesInGame[i].WaypointIndex = waypointIndices[i];

                if(EntityManager.EnemiesInGame[i].WaypointIndex == WaypointPositions.Length)
				{
                    EnqueueEnemyToRemove(EntityManager.EnemiesInGame[i]);
                    PlayerManager.ReceiveDamage(EntityManager.EnemiesInGame[i].MaxHealth);
                }

            }

            waypointsToUse.Dispose();
            waypointIndices.Dispose();
            enemySpeeds.Dispose();
            enemyAccess.Dispose();

            // Tick Towers

            foreach(TowerBehaviour tower in TowerManager.TowersInGame)
			{
                tower.Target = TowerTargeting.GetTarget(tower, TowerTargeting.TargetType.First);
                tower.Tick();
			}

            // Apply Effects

            // Damage Enemies

            if (damageData.Count > 0)
            {
                for (int i = 0; i < damageData.Count; i++)
                {
                    EnemyDamageData currentDamageData = damageData.Dequeue();
                    currentDamageData.TargetedEnemy.Health -= currentDamageData.TotalDamage / currentDamageData.Resistance;

                    if(currentDamageData.TargetedEnemy.Health <= 0f)
					{
                        EnqueueEnemyToRemove(currentDamageData.TargetedEnemy);
					}
                }
            }


            // Remove Enemies

            if (enemiesToRemove.Count > 0)
			{
                for (int i = 0; i < enemiesToRemove.Count; i++)
                {
                    EntityManager.RemoveEnemy(enemiesToRemove.Dequeue());
                }
            }

            // Remove Towers

            TowerManager.RemoveTowers();

            yield return null;
		}
	}

    public static void EnqueueDamageData(EnemyDamageData data)
	{
        damageData.Enqueue(data);
	}

    public static void EnqueueEnemyIDToSummon(int id)
	{
        enemyIDsToSummon.Enqueue(id);
	}

    public static void EnqueueEnemyToRemove(Enemy enemy)
	{
        enemiesToRemove.Enqueue(enemy);
    }

   
}

public struct MoveEnemiesJob: IJobParallelForTransform
{
    [NativeDisableParallelForRestriction]
    public NativeArray<int> WaypointIndex;

    [NativeDisableParallelForRestriction]
    public NativeArray<float> enemySpeeds;

    [NativeDisableParallelForRestriction]
    public NativeArray<Vector3> WaypointPositions;

    public float deltaTime;

    public void Execute(int index, TransformAccess transform)
	{
        if(WaypointIndex[index] < WaypointPositions.Length)
		{
            Vector3 positionToMoveTo = WaypointPositions[WaypointIndex[index]];
            transform.position = Vector3.MoveTowards(transform.position, positionToMoveTo, enemySpeeds[index] * deltaTime);

            if (transform.position == positionToMoveTo)
            {
                WaypointIndex[index]++;
            }
        }   
	}
}

public struct EnemyDamageData
{
    public EnemyDamageData(Enemy targetedEnemy, float totalDamage, float resistance)
	{
        TargetedEnemy = targetedEnemy;
        TotalDamage = totalDamage;
        Resistance = resistance;
	}

    public Enemy TargetedEnemy;
    public float TotalDamage;
    public float Resistance;
}