using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEditor;

public class PathController : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] public Path Path;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float lineWidth = 0.01f;

    private Dictionary<int, float> spawnDelays;
    public bool Activated;

	private void Awake()
	{
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        spawnDelays = new Dictionary<int, float>();
        for (int i = 0; i < 3; i++)
        {
            spawnDelays.Add(i, Path.enemyDelay[i]);
        }

        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;
        DrawPath();
    }


    public void Tick(int index)
    {
        if (!Activated) return;
        for (int i = 0; i < 3; i++)
        {
            if(spawnDelays[i] > 0f)
            {
                spawnDelays[i] -= Time.deltaTime;
                continue;
            }
            int enemyCount = MathUtils.GetScaledSpawn(Path.enemyCount[i], GameLoopManager.Instance.TimePassed); ;
            for (int j = 0; j < enemyCount; j++)
            {
                GameLoopManager.Instance.EnqueueEnemyToSummon(new EnemyCreateData() { EnemyId = i, PathIndex = index });
            }

            spawnDelays[i] = MathUtils.GetScaledDelay(Path.enemyDelay[i], GameLoopManager.Instance.TimePassed);
        }
    }

	// For editing
	public bool IsSelected = false;

	public void AddWaypoint(Vector3Int cell)
    {
        #if UNITY_EDITOR
            EditorUtility.SetDirty(Path);
        #endif  
        Path.waypoints.Add(cell);
    }


    public void RemoveWaypoint(Vector3Int cell)
    {
        #if UNITY_EDITOR
            EditorUtility.SetDirty(Path);
        #endif
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
        if (Path != null && IsSelected)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < Path.waypoints.Count; i++)
			{
                if(i > 0)
				{
                    DrawArrow.ForGizmo(GetCellCenter(Path.waypoints[i-1]), GetCellCenter(Path.waypoints[i]));
                }
                Gizmos.DrawWireSphere(GetCellCenter(Path.waypoints[i]), 0.05f);
			}
        }
    }

    public void DrawPath()
    {
        if(Path != null && lineRenderer != null)
        {
            for (int i = 0; i < Path.waypoints.Count; i++)
            {
                lineRenderer.positionCount++;
                Vector3 position = GetCellCenter(Path.waypoints[i]);
                position.z = 200;
                lineRenderer.SetPosition(i, position);
            }
        }
    }

    public void ToggleHighlight(bool on)
    {
        lineRenderer.enabled = on;
    }

    private void OnDrawGizmosSelected()
    {
        if (Path != null)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < Path.waypoints.Count; i++)
            {
                if (i > 0)
                {
                    DrawArrow.ForGizmo(GetCellCenter(Path.waypoints[i - 1]), GetCellCenter(Path.waypoints[i]));
                }
                Gizmos.DrawWireSphere(GetCellCenter(Path.waypoints[i]), 0.05f);
            }
        }
    }
}
