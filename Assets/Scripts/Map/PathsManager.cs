using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathsManager : MonoBehaviour
{
	[SerializeField] private Tilemap baseMap;
	[SerializeField] private Grid grid;
	private static List<Tilemap> tilemaps;
	private static List<PathController> pathControllers;
	private static GameObject pathsParent;

	// For Game Loop
	public static int MaxIndex;
	public static int PathsCount;
	public static int[] PathSizes;
	public static Vector3[][] WaypointPositions;
	public static int TotalWaypoints;
	public static float[][] WaypointDistances;
	public static float[][] WaypointDistancesToEnd;

	// For editing
	public static int editingPathIndex;
	public static PathSelector selector;


	// Start is called before the first frame update
	public static void Init()
	{
		tilemaps = new List<Tilemap>();
		pathControllers = new List<PathController>();
		pathsParent = GameObject.Find("Grid/Paths");
		LoadPaths();
		CalculateWaypoints();

		if (GameState.Instance.IsEditing)
		{
			selector = GameObject.Find("UI Canvas/PathEditor/PathSelector").GetComponent<PathSelector>();
			InitializeSelector();
		}
	}

	public static void ActivatedPath(int index)
    {
		pathControllers[index].Activated = true;
    }

	public static void HightlightPath(int index)
    {
		pathControllers[index].ToggleHighlight(true);
    }

	public static void DisableHighlight()
    {
        for (int i = 0; i < pathControllers.Count; i++)
        {
			pathControllers[i].ToggleHighlight(false);
        }
    }

	private static void CalculateWaypoints()
	{
		PathsCount = pathControllers.Count;
		WaypointPositions = new Vector3[pathControllers.Count][];
		WaypointDistances = new float[pathControllers.Count][];
		WaypointDistancesToEnd = new float[pathControllers.Count][];
		PathSizes = new int[pathControllers.Count];
		MaxIndex = 0;
		for (int i = 0; i < pathControllers.Count; i++)
		{
			PathSizes[i] = pathControllers[i].Path.waypoints.Count;
			TotalWaypoints += PathSizes[i];
			WaypointPositions[i] = pathControllers[i].GetWorldArray();
			WaypointDistances[i] = new float[Mathf.Max(0, WaypointPositions[i].Length - 1)];
			if (pathControllers[i].Path.waypoints.Count * PathsCount + i > MaxIndex)
			{
				MaxIndex = pathControllers[i].Path.waypoints.Count * PathsCount + i;
			}
			for (int j = 0; j < WaypointPositions[i].Length - 1; j++)
			{
				WaypointDistances[i][j] = Vector3.Distance(WaypointPositions[i][j], WaypointPositions[i][j + 1]);
			}
			WaypointDistancesToEnd[i] = new float[WaypointPositions[i].Length];
			for (int j = 0; j < WaypointDistances[i].Length; j++)
			{
				float distance = 0;
				for (int k = j; k < WaypointDistances[i].Length; k++)
				{
					distance += WaypointDistances[i][k];
				}
				WaypointDistancesToEnd[i][j] = distance;
			}
		}
	}

	public static void TickPaths() 
	{
        for (int i = 0; i < PathsCount; i++)
        {
			pathControllers[i].Tick(i);
		}
	}

	private static void LoadPaths()
	{
		foreach (Tilemap tilemap in pathsParent.transform.GetComponentsInChildren<Tilemap>())
		{
			tilemaps.Add(tilemap);
		}
		foreach (PathController pathController in pathsParent.transform.GetComponentsInChildren<PathController>())
		{
			pathControllers.Add(pathController);
		}
	}


	private void Update()
	{
		if (InputManager.IsPointerOverUIObject() || !GameState.Instance.IsEditing)
		{
			return;
		}

		Vector3Int mousePos = GridController.GetMousePosition(grid, baseMap);
		// Left-click -> place waypoint
		if (Input.GetMouseButtonUp(0))
		{
			pathControllers[editingPathIndex].AddWaypoint(mousePos);
		}

		// Right-click -> delete waypoint
		if (Input.GetMouseButtonUp(1))
		{
			pathControllers[editingPathIndex].RemoveWaypoint(mousePos);
		}
	}

	private static void OnChangeSelectedPath(int index)
	{
		pathControllers[editingPathIndex].IsSelected = false;
		editingPathIndex = index;
		pathControllers[editingPathIndex].IsSelected = true;
	}

	private static void InitializeSelector()
	{
		editingPathIndex = 0;
		pathControllers[0].IsSelected = true;
		string[] pathNames = new string[pathControllers.Count];
		for (int i = 0; i < pathNames.Length; i++)
		{
			pathNames[i] = pathControllers[i].Path.name;
		}
		selector.RegisterPaths(pathNames, OnChangeSelectedPath);
	}
}
