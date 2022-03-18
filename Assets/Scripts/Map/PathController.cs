using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

[ExecuteInEditMode]
public class PathController : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap = null;
    [SerializeField] private Path path = null;
    [SerializeField] private Tile flagTile = null;


    private void Awake()
    {
        RenderWaypoints();
    }

    void Start()
    {
        tileMap = gameObject.GetComponent<Tilemap>();
        RenderWaypoints();
    }

    public void AddWaypoint(Vector3Int cell)
    {
        path.waypoints.Add(cell);
        RenderWaypoints();
    }


    public void RemoveWaypoint(Vector3Int cell)
    {
        path.waypoints.Remove(cell);
        RenderWaypoints();

    }

    private Vector3 GetCellCenter(Vector3Int cell)
    {
        if (tileMap != null)
        {
            Vector3 worldPosition = tileMap.CellToWorld(cell);
            worldPosition.y += 0.16f;
            return worldPosition;
        }
        return cell;
    }

    private void RenderWaypoints()
    {
        // disabling for now
        return;

        if (path != null && tileMap != null && Debug.isDebugBuild)
        {
            tileMap.ClearAllTiles();
            foreach (Vector3Int waypoint in path.waypoints)
            {
                tileMap.SetTile(waypoint, flagTile);
            }
        }
    }

    public Vector3[] GetWorldArray()
    {
        Vector3[] positions = new Vector3[path.waypoints.Count];
		for (int i = 0; i < path.waypoints.Count; i++)
		{
            positions[i] = tileMap.CellToWorld(path.waypoints[i]);
            positions[i].y += GlobalConstants.TILE_HEIGHT;
            positions[i].z = 2;

        }
        return positions;
    }

    private void OnDrawGizmosSelected()
    {
        if (path != null && path.waypoints.Count > 1)
        {

            Vector3Int last = path.waypoints[0];
            foreach (Vector3Int waypoint in path.waypoints)
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
