using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;


[CreateAssetMenu(fileName = "New Generator", menuName = "ScriptableObjects/Generator")]
public class GeneratorData : ScriptableObject
{
    public GameObject GeneratorPrefab;
    public int GeneratorID;
}