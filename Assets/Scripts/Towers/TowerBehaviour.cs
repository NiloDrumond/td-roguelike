using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehaviour: MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite level1;
    [SerializeField] private Sprite level2;
    [SerializeField] private Sprite level3;
    public LayerMask EnemiesLayer;
    public Enemy Target;
    public float Damage;
    public float Firerate;
    public float Range;
    public int CostToUpgrade;
    public float level;

    private IDamageMethod damageMethod;

    private void Start()
	{
        IDamageMethod method = GetComponent<IDamageMethod>();

        if(method == null)
		{
            Debug.LogError("TOWER_BEHAVIOUR: Damage method not found");
		}

        damageMethod = method;
        spriteRenderer.sprite = level1;
        damageMethod.Init(Damage, Firerate);
        level = 1;
	}

    public void Upgrade()
    {
        IDamageMethod method = GetComponent<IDamageMethod>();
        if (level == 3) return;
        if (method == null)
        {
            Debug.LogError("TOWER_BEHAVIOUR: Damage method not found");
        }
        level = level+1;
        damageMethod = method;
        if (level == 2)
        {
            spriteRenderer.sprite = level2;
        }
        else
        {
            spriteRenderer.sprite = level3;
        }
        damageMethod.upgrade(Damage, Firerate);


    }

    public void Tick()
	{
        if(damageMethod != null)
		{
            damageMethod.DamageTick(Target);
		}
	}
}
