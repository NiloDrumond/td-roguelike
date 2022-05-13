using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RegionsManager : MonoBehaviour
{
	private static List<RegionController> regionControllers;
	private static List<Tilemap> regionTilemaps;
	private static GameObject regionsParent;
	// Start is called before the first frame update

	private static bool isInitialized = false;

	public static void Init()
    {
		if(!isInitialized)
        {
			regionControllers = new List<RegionController>();
			regionTilemaps = new List<Tilemap>(); 

			regionsParent = GameObject.Find("Grid/Regions");
			LoadRegions();
			isInitialized = true;
        }
    }

	public static void ActivateRegion(int index) 
	{
		// Activate Region
		regionControllers[index].Activated = true;
		// Activate Path
		PathsManager.ActivatedPath(index);
		// Activate Generators

		GameState.Instance.IsUnlockingRegion = false;
		GameState.Instance.UnlockedRegions++;
		PlayerManager.AddSupplies(new Supplies(100 * GameState.Instance.UnlockedRegions));
		if (GameState.Instance.UnlockedRegions == regionControllers.Count)
        {
			GameState.Instance.AllRegionsUnlocked = true;
        }
		GameLoopManager.Instance.StartLoop();
	}

	public static void HighlightRegion(int index)
    {
		regionControllers[index].ToggleHightlight(true);
		PathsManager.HightlightPath(index);
    }

	public static void DisableHighlight()
    {
        for (int i = 0; i < regionControllers.Count; i++)
        {
			regionControllers[i].ToggleHightlight(false);
		}
		PathsManager.DisableHighlight();
	}

	public static bool CheckForPlacement(Vector3Int position) 
	{
        for (int i = 0; i < regionTilemaps.Count; i++)
        {
			var tile = regionTilemaps[i].GetTile(position);
			if(tile != null)
            {
				var activated = regionControllers[i].Activated;
				if (activated) return true;
            }
        }
		return false;
	}

	public static int GetUnlockableRegion(Vector3Int position)
    {
		for (int i = 0; i < regionTilemaps.Count; i++)
		{
			var tile = regionTilemaps[i].GetTile(position);
			if (tile != null)
			{
				return regionControllers[i].Activated ? -1 : i;
			}
		}
		return -1;
	}

	private static void LoadRegions()
	{
		foreach (RegionController regionController in regionsParent.transform.GetComponentsInChildren<RegionController>())
		{
			regionControllers.Add(regionController);
		}
		foreach (Tilemap regionTilemap in regionsParent.transform.GetComponentsInChildren<Tilemap>())
		{
			regionTilemaps.Add(regionTilemap);
		}
	}

}
