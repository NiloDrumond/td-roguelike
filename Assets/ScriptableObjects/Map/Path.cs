using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Path", menuName = "ScriptableObjects/Path")]
public class Path : ScriptableObject
{
    public List<Vector3Int> waypoints;
}
