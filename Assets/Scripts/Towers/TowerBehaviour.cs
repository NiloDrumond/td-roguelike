using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehaviour
{
    public LayerMask EnemiesLayer;
    public Enemy Target;
    public TowerData Data;
    public Vector3Int Position;
    public Vector3 WorldPosition;
    public float Range;

    private float delay;

    

    // Start is called before the first frame update
    public TowerBehaviour(TowerData data, Vector3Int position, Vector3 worldPosition)
	{
        Data = data;
        Position = position;
        WorldPosition = worldPosition;

        Range = data.BaseRange * (1 + GlobalConstants.TOWER_RANGE_HEIGHT_BONUS * position.z);
        EnemiesLayer = data.EnemiesLayer;
        delay = 1 / data.BaseFirerate;

        Data.DamageMethod.Init(data.BaseDamage, data.BaseFirerate);
    }

	public void Tick()
	{
        

        if(Target != null)
		{
            Data.DamageMethod.DamageTick(Target);
		}
	}
}
