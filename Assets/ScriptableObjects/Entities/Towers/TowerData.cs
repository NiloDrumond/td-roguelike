using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;


[CreateAssetMenu(fileName = "New Tower", menuName = "ScriptableObjects/Tower")]
public class TowerData : ScriptableObject
{
    public LayerMask EnemiesLayer;
    public Tile Tile;
    public float BaseRange;
    public float BaseDamage;
    public float BaseFirerate;
    [SerializeReference] public IDamageMethod DamageMethod;
}
