using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionCollisionManager : MonoBehaviour
{
	[SerializeField] private TowerBehaviour behaviour;
	[SerializeField] private MissileDamage damage;
	[SerializeField] private ParticleSystem explosionSystem;
	[SerializeField] private ParticleSystem missileSystem;
	[SerializeField] private float explosionRadius;
	private List<ParticleCollisionEvent> missileCollisions;

	private void Start()
	{
		missileCollisions = new List<ParticleCollisionEvent>();
	}

	private void OnParticleCollision(GameObject other)
	{
		missileSystem.GetCollisionEvents(other, missileCollisions);

		for (int i = 0; i < missileCollisions.Count; i++)
		{
			explosionSystem.transform.position = missileCollisions[i].intersection;
			explosionSystem.Play();

			Collider2D[] enemiesInRadius = Physics2D.OverlapCircleAll(missileCollisions[i].intersection, explosionRadius, behaviour.EnemiesLayer);

			for (int j = 0; j < enemiesInRadius.Length; j++)
			{
				Enemy enemyToDamage = EntityManager.EnemyTransformMap[enemiesInRadius[i].transform];
				EnemyDamageData damageToApply = new EnemyDamageData(enemyToDamage, damage.Damage, enemyToDamage.DamageResistance);
				GameLoopManager.Instance.EnqueueDamageData(damageToApply);
			}

		}
	}
}
