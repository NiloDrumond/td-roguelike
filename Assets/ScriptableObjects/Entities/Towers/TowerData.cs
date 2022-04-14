using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;


[CreateAssetMenu(fileName = "New Tower", menuName = "ScriptableObjects/Tower")]
public class TowerData : ScriptableObject
{
    public GameObject TowerPrefab;
    public int TowerID;
    public int MineralsCost;
}
