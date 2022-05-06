using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDamage : MonoBehaviour, IDamageMethod
{
	[SerializeField] private Transform LaserPivot;
	[SerializeField] private LineRenderer LaserRenderer;

	private float damage;
	private float firerate;
	private float delay;

	public void DamageTick(Enemy target)
	{
		if(target != null )
		{
			LaserRenderer.SetPosition(0, LaserPivot.position);
			LaserRenderer.SetPosition(1, target.transform.position);
			LaserRenderer.enabled = true;

			if (delay > 0f)
			{
				delay -= Time.deltaTime;
				return;
			}

			GameLoopManager.Instance.EnqueueDamageData(new EnemyDamageData(target, damage, target.DamageResistance));

			delay = 1 / firerate;
		} else
		{
			LaserRenderer.enabled = false;	
		}
		
	}

	public void Init(float damage, float firerate)
	{
		this.damage = damage;
		this.firerate = firerate;
		this.delay = 1 / firerate;
		LaserRenderer.useWorldSpace = true;
	}
		public void upgrade(float damage, float firerate)
		{
			this.firerate = firerate*3;
		}
}
