using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageMethod
{
    public void DamageTick(Enemy target);
    public void Init(float damage, float firerate);
}

[Serializable]
public class StandardDamage : IDamageMethod
{
	private float damage;
	private float firerate;
	private float delay;

	public void DamageTick(Enemy target)
	{
		if(delay > 0f)
		{
			delay -= Time.deltaTime;
			return;
		}

		GameLoopManager.EnqueueDamageData(new EnemyDamageData(target, damage, target.DamageResistance));

		delay = 1 / firerate;
	}

	public void Init(float damage, float firerate)
	{
		this.damage = damage;
		this.firerate = firerate;
		this.delay = 1 / firerate;
	}
}
