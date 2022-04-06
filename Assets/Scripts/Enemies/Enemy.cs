using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	public int WaypointIndex;
	public int PathIndex;
	public float DamageResistance = 1f;
	public float MaxHealth;
	public float Health;
	public float Speed;
	public int ID;

    public void Init(int pathIndex)
	{
		Health = MaxHealth;
		PathIndex = pathIndex;
		transform.position = PathsManager.WaypointPositions[pathIndex][0];
		WaypointIndex = 0;
	}

}
