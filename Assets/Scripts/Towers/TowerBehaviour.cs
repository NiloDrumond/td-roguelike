using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehaviour: MonoBehaviour
{
    [SerializeField] private SpriteRenderer tSpriteRenderer;
    [SerializeField] private Sprite Llevel1;
    [SerializeField] private Sprite Llevel2;
    [SerializeField] private Sprite Llevel3;
    public LayerMask EnemiesLayer;
    public Enemy Target;
    public float Damage;
    public float Firerate;
    public float Range;
    private float delay;
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
        tSpriteRenderer.sprite = Llevel1;
        damageMethod.Init(Damage, Firerate);
        level = 1;
        delay = 1 / Firerate;
	}

    public void Upgrade()
    {
        IDamageMethod method = GetComponent<IDamageMethod>();

        if (method == null)
        {
            Debug.LogError("TOWER_BEHAVIOUR: Damage method not found");
        }
        level = level+1;
        damageMethod = method;
        if (level == 2)
        {
            tSpriteRenderer.sprite = Llevel2;
        }
        else
        {
            tSpriteRenderer.sprite = Llevel3;
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
