using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileDamage : MonoBehaviour, IDamageMethod
{
	[SerializeField] private ParticleSystem missileSystem;
	[SerializeField] private Transform towerHead;

	private ParticleSystem.MainModule missileSystemMain;

	public float Damage;
	private float firerate;
	private float delay;

	public void DamageTick(Enemy target)
	{
		if (delay > 0f)
		{
			delay -= Time.deltaTime;
			return;
		}

		missileSystemMain.startRotationX = target.transform.position.x - towerHead.transform.forward.x;
		missileSystemMain.startRotationY = target.transform.position.y - towerHead.transform.forward.y;
		missileSystemMain.startRotationZ = target.transform.position.z - towerHead.transform.forward.z;

		missileSystem.Play();

		delay = 1 / firerate;
	}

	public void Init(float damage, float firerate)
	{
		missileSystemMain = missileSystem.main;
		this.Damage = damage;
		this.firerate = firerate;
		this.delay = 1 / firerate;
	}
}
