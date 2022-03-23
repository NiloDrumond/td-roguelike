using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class TowerTargeting
{
    public enum TargetType
	{
		First,
		Last,
		Close
	}

	private static float GetCompareValue(TargetType targetingType)
	{
		switch(targetingType)
		{
			case TargetType.Last:
				return Mathf.NegativeInfinity;
			case TargetType.First:
			case TargetType.Close:
				return Mathf.Infinity;
			default:
				return 0;
		}
	}

    public static Enemy GetTarget(TowerBehaviour currentTower, TargetType targetingType)
	{
		Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(new Vector3(currentTower.WorldPosition.x, currentTower.WorldPosition.y, 0), currentTower.Range, currentTower.EnemiesLayer);
		NativeArray<EnemyData> enemiesToCalculate = new NativeArray<EnemyData>(enemiesInRange.Length, Allocator.TempJob);
		NativeArray<int> enemyToIndex = new NativeArray<int>(new int[] { -1 }, Allocator.TempJob);


		int waypointsMaxIndex = GameLoopManager.WaypointPositions.Length - 1;

		for (int i = 0; i < enemiesToCalculate.Length; i++)
		{
			Enemy enemy = enemiesInRange[i].GetComponent<Enemy>();
			int enemyIndexInList = EntitySummoner.EnemiesInGame.FindIndex(x => x == enemy);
			
			// Mathf.Min for the case when the enemy reaches the end making its waypointIndex == waypoints.length
			float distanceToEnd = GameLoopManager.WaypointDistancesToEnd[Mathf.Min(enemy.WaypointIndex, waypointsMaxIndex)];
			Vector3 waypointPosition = GameLoopManager.WaypointPositions[Mathf.Min(enemy.WaypointIndex, waypointsMaxIndex)];

			enemiesToCalculate[i] = new EnemyData(new Vector3(enemy.transform.position.x, enemy.transform.position.y, 0), enemy.WaypointIndex, enemy.Health, enemyIndexInList, distanceToEnd, waypointPosition);
		}

		SearchForEnemy enemySearchJob = new SearchForEnemy(enemiesToCalculate, enemyToIndex, new Vector3(currentTower.WorldPosition.x, currentTower.WorldPosition.y, 0), GetCompareValue(targetingType), (int)targetingType);

		JobHandle dependency = new JobHandle();
		JobHandle SearchJobHandle = enemySearchJob.Schedule(enemiesToCalculate.Length, dependency);

		SearchJobHandle.Complete();
		
		

		// No enemies found
		if (enemyToIndex[0] == -1)
		{
			enemyToIndex.Dispose();
			enemiesToCalculate.Dispose();
			return null;
		} else
		{
			int enemyIndexToReturn = enemiesToCalculate[enemyToIndex[0]].EnemyIndex;
			enemyToIndex.Dispose();
			enemiesToCalculate.Dispose();

			return EntitySummoner.EnemiesInGame[enemyIndexToReturn];
		}

		
	}

	struct EnemyData
	{
		public EnemyData(Vector3 position, int waypointIndex, float health, int enemyIndex, float distanceToEnd, Vector3 nextWaypointPosition)
		{
			Position = position;
			WaypointIndex = waypointIndex;
			Health = health;
			EnemyIndex = enemyIndex;
			DistanceToEnd = distanceToEnd;
			NextWaypointPosition = nextWaypointPosition;
		}

		public Vector3 Position;
		public int WaypointIndex;
		public float Health;
		public int EnemyIndex;
		public float DistanceToEnd;
		public Vector3 NextWaypointPosition;
	}

	struct SearchForEnemy : IJobFor
	{
		public readonly NativeArray<EnemyData> _enemiesToCalculate;
		public NativeArray<int> _enemyToIndex;
		public readonly Vector3 TowerPosition;
		public float CompareValue;
		public int TargetingType;

		public SearchForEnemy(NativeArray<EnemyData> enemiesToCalculate, NativeArray<int> enemyToIndex, Vector3 towerPosition, float compareValue, int targetingType)
		{
			_enemiesToCalculate = enemiesToCalculate;
			_enemyToIndex = enemyToIndex;
			TowerPosition = towerPosition;
			CompareValue = compareValue;
			TargetingType = targetingType;

		}

		public void Execute(int index)
		{
			float currentEnemyDistanceToEnd = 0;
			float distanceToEnemy = 0;
			switch (TargetingType)
			{
				case 0: //First
					currentEnemyDistanceToEnd = GetDistanceToEnd(_enemiesToCalculate[index]);
					if (currentEnemyDistanceToEnd < CompareValue)
					{
						_enemyToIndex[0] = index;
						CompareValue = currentEnemyDistanceToEnd;
					}

					break;
				case 1: // Last
					currentEnemyDistanceToEnd = GetDistanceToEnd(_enemiesToCalculate[index]);
					if (currentEnemyDistanceToEnd > CompareValue)
					{
						_enemyToIndex[0] = index;
						CompareValue = currentEnemyDistanceToEnd;
					}

					break;
				case 2: // Close
					distanceToEnemy = Vector3.Distance(TowerPosition, _enemiesToCalculate[index].Position);
					if (distanceToEnemy < CompareValue)
					{
						_enemyToIndex[0] = index;
						CompareValue = distanceToEnemy;
					}
					break;
			}
		}

		private float GetDistanceToEnd(EnemyData enemy)
		{
			return enemy.DistanceToEnd + Vector3.Distance(enemy.NextWaypointPosition, enemy.Position);
		}
	}
}
