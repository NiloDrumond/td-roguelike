using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	public int WaypointIndex;

	public float DamageResistance = 1f;
	public float MaxHealth;
	public float Health;
	public float Speed;
	public int ID;

    public void Init()
	{
		Health = MaxHealth;
		transform.position = GameLoopManager.WaypointPositions[0];
		WaypointIndex = 0;
	}

}
