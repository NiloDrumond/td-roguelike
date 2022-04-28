using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Path", menuName = "ScriptableObjects/Path")]
public class Path : ScriptableObject
{
    public List<Vector3Int> waypoints;
    public float[] enemyDelay = new float[3] {5,5,5};
    public int[] enemyCount = new int[3] {0,0,0};
}
