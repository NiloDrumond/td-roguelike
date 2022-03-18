using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New EnemySummon", menuName = "ScriptableObjects/EnemySummon")]
public class EnemySummon : ScriptableObject
{
    public GameObject EnemyPrefab;
    public int EnemyID;
}
