using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehaviour: MonoBehaviour
{
    public LayerMask EnemiesLayer;
    public Enemy Target;
    public float Damage;
    public float Firerate;
    public float Range;

    private float delay;

    private IDamageMethod damageMethod;

    private void Start()
	{
        IDamageMethod method = GetComponent<IDamageMethod>();

        if(method == null)
		{
            Debug.LogError("TOWER_BEHAVIOUR: Damage method not found");
		}

        damageMethod = method;

        damageMethod.Init(Damage, Firerate);

        delay = 1 / Firerate;
	}


	public void Tick()
	{
        if(damageMethod != null)
		{
            damageMethod.DamageTick(Target);
		}
	}

	private void OnDrawGizmos()
	{
        Gizmos.DrawWireSphere(gameObject.transform.position, Range);
	}
}
