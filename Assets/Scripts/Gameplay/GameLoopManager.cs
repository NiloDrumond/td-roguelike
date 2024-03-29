using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UI;

public class GameLoopManager : MonoBehaviour
{
    public static GameLoopManager Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }


    private Queue<EnemyDamageData> damageData;

    private Queue<Enemy> enemiesToRemove;
    private Queue<EnemyCreateData> enemiesToSummon;

    public bool IsRunning;
    public float TimePassed;

    // Start is called before the first frame update
    void Start()
    {
        damageData = new Queue<EnemyDamageData>();
        enemiesToSummon = new Queue<EnemyCreateData>();
        enemiesToRemove = new Queue<Enemy>();

        RegionsManager.Init();
        PathsManager.Init();
        PlayerManager.Init();
        EntityManager.Init();
        TowerManager.Init();
        GeneratorManager.Init();
        InputManager.Init();

        if (GameState.Instance.IsEditing) return;
        TimePassed = 0;
        StartLoop();
    }   
    private void Update()
    {
        if(IsRunning)
        {
            TimePassed += Time.deltaTime;
            if(!GameState.Instance.AllRegionsUnlocked && TimePassed/20 > GameState.Instance.UnlockedRegions)
            {
                GameState.Instance.IsUnlockingRegion = true;
                StopLoop();
            }
        }
    }


    public void StartLoop()
    {
        IsRunning = true;
        StartCoroutine(GameLoop());
    }

    public void StopLoop()
    {
        IsRunning = false;
    }


    // Update is called once per frame
    IEnumerator GameLoop()
	{
        while(IsRunning)
		{
            // Tick Paths to spawn Enemies

            PathsManager.TickPaths();

            // Spawn Enemies
            
            if(enemiesToSummon.Count > 0)
			{
                for(int i = 0; i < enemiesToSummon.Count; i++)
				{
                    EntityManager.SummonEnemy(enemiesToSummon.Dequeue());
				}
			}

            // Spawn Supplies

            GeneratorManager.SpawnGenerators();

            // Tick Supplies

            foreach(GeneratorBehaviour generator in GeneratorManager.GeneratorInGame)
			{
                generator.Tick();
			}

            // Spawn Towers

            TowerManager.SpawnTowers();

            // Tick Towers

            foreach (TowerBehaviour tower in TowerManager.TowersInGame)
            {
                tower.Target = TowerTargeting.GetTarget(tower, TowerTargeting.TargetType.First);
                tower.Tick();
            }

            // Damage Enemies

            if (damageData.Count > 0)
            {
                for (int i = 0; i < damageData.Count; i++)
                {
                    EnemyDamageData currentDamageData = damageData.Dequeue();
                    currentDamageData.TargetedEnemy.Health -= currentDamageData.TotalDamage / currentDamageData.Resistance;

                    if (currentDamageData.TargetedEnemy.Health <= 0f)
                    {
                        EnqueueEnemyToRemove(currentDamageData.TargetedEnemy);
                    }
                }
            }

            // Move Enemies
            NativeArray<Vector3> waypointPositions = new NativeArray<Vector3>(PathsManager.MaxIndex  + 1, Allocator.TempJob);
            NativeArray<int> waypointIndices = new NativeArray<int>(EntityManager.EnemiesInGame.Count, Allocator.TempJob);
            NativeArray<int> pathIndices = new NativeArray<int>(EntityManager.EnemiesInGame.Count, Allocator.TempJob);
            NativeArray<float> enemySpeeds = new NativeArray<float>(EntityManager.EnemiesInGame.Count, Allocator.TempJob);
            TransformAccessArray enemyAccess = new TransformAccessArray(EntityManager.EnemiesInGameTransform.ToArray(), 2);

			for (int i = 0; i < PathsManager.WaypointPositions.Length; i++)
			{
				for (int j = 0; j < PathsManager.WaypointPositions[i].Length; j++)
				{
                    waypointPositions[j * PathsManager.WaypointPositions.Length + i] = PathsManager.WaypointPositions[i][j];
                }
            }

			for (int i = 0; i < EntityManager.EnemiesInGame.Count; i++)
			{
                enemySpeeds[i] = EntityManager.EnemiesInGame[i].Speed;
                waypointIndices[i] = EntityManager.EnemiesInGame[i].WaypointIndex;
                pathIndices[i] = EntityManager.EnemiesInGame[i].PathIndex;

            }

            MoveEnemiesJob moveJob = new MoveEnemiesJob {
                enemySpeeds = enemySpeeds,
                WaypointIndices = waypointIndices,
                WaypointPositions = waypointPositions,
                deltaTime = Time.deltaTime,
                PathIndices = pathIndices,
                PathsCount = PathsManager.PathsCount
            };

            JobHandle moveJobHandle = moveJob.Schedule(enemyAccess);
            moveJobHandle.Complete();

			for (int i = 0; i < EntityManager.EnemiesInGame.Count; i++)
			{
                Enemy enemy = EntityManager.EnemiesInGame[i];
                enemy.WaypointIndex = waypointIndices[i];

                if(enemy.WaypointIndex == PathsManager.PathSizes[enemy.PathIndex])
				{
                    EnqueueEnemyToRemove(enemy);
                    PlayerManager.ReceiveDamage(enemy.MaxHealth);
                }

            }

            pathIndices.Dispose();
            waypointPositions.Dispose();
            waypointIndices.Dispose();
            enemySpeeds.Dispose();
            enemyAccess.Dispose();

            // Apply Effects

            // Remove Enemies

            if (enemiesToRemove.Count > 0)
			{
                for (int i = 0; i < enemiesToRemove.Count; i++)
                {
                    EntityManager.RemoveEnemy(enemiesToRemove.Dequeue());
                }
            }

            // Remove Generators

            GeneratorManager.RemoveGenerators();

            // Remove Towers

            TowerManager.RemoveTowers();

            yield return null;
		}
    }

    public void EnqueueDamageData(EnemyDamageData data)
	{
        damageData.Enqueue(data);
	}

    public void EnqueueEnemyToSummon(EnemyCreateData data)
	{
        enemiesToSummon.Enqueue(data);
	}

    public void EnqueueEnemyToRemove(Enemy enemy)
	{
        enemiesToRemove.Enqueue(enemy);
    }

}

public struct MoveEnemiesJob: IJobParallelForTransform
{
    [NativeDisableParallelForRestriction]
    public NativeArray<int> WaypointIndices;

    [NativeDisableParallelForRestriction]
    public NativeArray<int> PathIndices;

    [NativeDisableParallelForRestriction]
    public NativeArray<float> enemySpeeds;

    [NativeDisableParallelForRestriction]
    public NativeArray<Vector3> WaypointPositions;

    [NativeDisableParallelForRestriction]
    public int PathsCount;


    public float deltaTime;

    public void Execute(int index, TransformAccess transform)
	{
        if (PathIndices[index] + PathsCount * WaypointIndices[index] < WaypointPositions.Length)
		{
            Vector3 positionToMoveTo = WaypointPositions[PathIndices[index] + PathsCount * WaypointIndices[index]];
            transform.position = Vector3.MoveTowards(transform.position, positionToMoveTo, enemySpeeds[index] * deltaTime);

            if (transform.position == positionToMoveTo)
            {
                WaypointIndices[index]++;
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