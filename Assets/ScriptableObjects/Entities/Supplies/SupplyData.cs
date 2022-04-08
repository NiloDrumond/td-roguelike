using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;


[CreateAssetMenu(fileName = "New Supply", menuName = "ScriptableObjects/Supply")]
public class SupplyData : ScriptableObject
{
    public GameObject SupplyPrefab;
    public int SupplyID;
}