using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultPath", menuName = "ScriptableObjects/Paths")]
public class Path : ScriptableObject
{
    public List<Vector3Int> waypoints;
}
