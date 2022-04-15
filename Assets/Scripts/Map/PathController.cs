using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEditor;

public class PathController : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] public Path Path;

	private void Awake()
	{
        grid = GameObject.Find("Grid").GetComponent<Grid>();
	}

	// For editing
	public bool IsSelected = false;

	public void AddWaypoint(Vector3Int cell)
    {
        EditorUtility.SetDirty(Path);
        Path.waypoints.Add(cell);
    }


    public void RemoveWaypoint(Vector3Int cell)
    {
        EditorUtility.SetDirty(Path);
        Path.waypoints.Remove(cell);
    }

    private Vector3 GetCellCenter(Vector3Int cell)
    {
        if (grid != null)
        {
            Vector3 worldPosition = grid.CellToWorld(cell);
            worldPosition.y += 0.16f;
            return worldPosition;
        }
        return cell;
    }

    public Vector3[] GetWorldArray()
    {
        Vector3[] positions = new Vector3[Path.waypoints.Count];
		for (int i = 0; i < Path.waypoints.Count; i++)
		{
            positions[i] = grid.CellToWorld(Path.waypoints[i]);
            positions[i].y += GlobalConstants.TILE_HEIGHT;
            positions[i].z = 2;

        }
        return positions;
    }

    private void OnDrawGizmos()
    {
        if (Path != null && Path.waypoints.Count > 1 && IsSelected)
        {

            Vector3Int last = Path.waypoints[0];
            foreach (Vector3Int waypoint in Path.waypoints)
            {
                // using if to avoid first case
                if (!waypoint.Equals(last))
                {
                    Gizmos.color = Color.blue;
                    DrawArrow.ForGizmo(GetCellCenter(last), GetCellCenter(waypoint));
                    last = waypoint;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (Path != null && Path.waypoints.Count > 1)
        {

            Vector3Int last = Path.waypoints[0];
            foreach (Vector3Int waypoint in Path.waypoints)
            {
                // using if to avoid first case
                if (!waypoint.Equals(last))
                {
                    Gizmos.color = Color.blue;
                    DrawArrow.ForGizmo(GetCellCenter(last), GetCellCenter(waypoint));
                    last = waypoint;
                }
            }
        }
    }
}
