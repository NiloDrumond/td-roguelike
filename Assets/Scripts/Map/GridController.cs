using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GridController : MonoBehaviour
{
	[SerializeField] private Grid grid = null;
	[SerializeField] private Tilemap baseMap = null;
	[SerializeField] private Tilemap interactiveMap = null;
	[SerializeField] private Tilemap towersMap = null;
	[SerializeField] private Tilemap suppliesMap = null;
	[SerializeField] private IsometricRuleTile hoverTile = null;
	[SerializeField] private IsometricRuleTile highHoverTile = null;
	[SerializeField] private Tile EmptyTile = null;

	private Vector3Int previousCursorPosition = new Vector3Int();
	private int score = 0;


	private void OnMouseOver()
	{
		if (InputManager.IsPointerOverUIObject())
		{
			return;
		}

		// Mouse over -> highlight tile
		Vector3Int mousePos = GetMousePosition(grid, baseMap);
		if (!mousePos.Equals(previousCursorPosition))
		{
			var top = GetTopTile(mousePos);
			Vector3Int pos = top.First;
			bool isHigh = top.Second;
			Vector3Int high = new Vector3Int(pos.x - 100, pos.y - 100, pos.z + 100);
			interactiveMap.SetTile(previousCursorPosition, null); // Remove old hoverTile
			interactiveMap.SetTile(high, isHigh ? highHoverTile : hoverTile);
			previousCursorPosition = high;
		}

		// Left-click -> place tower
		if (Input.GetMouseButtonUp(0) && !GameState.Instance.IsEditing)
		{
			var top = GetTopTile(mousePos);
			Debug.Log(mousePos);
			Debug.Log(top.First);
			Vector3Int pos = top.First;
			pos.z += 1;
			var tile = towersMap.GetTile(pos);

			if (tile == null)
			{
				Vector3 worldPosition = towersMap.CellToWorld(pos);
				bool success = TowerManager.PlaceTower(new Vector3(worldPosition.x, worldPosition.y, pos.z));
				if (success)
				{
					towersMap.SetTile(pos, EmptyTile);
				}
			}
		}

		// Middle-click -> place supply
		if (Input.GetMouseButtonUp(2) && !GameState.Instance.IsEditing)
		{
			var top = GetTopTile(mousePos);
			Vector3Int pos = top.First;
			pos.z += 1;
			var tile = suppliesMap.GetTile(pos);
			if (tile == null)
			{
				Vector3 worldPosition = suppliesMap.CellToWorld(pos);
				bool success = GeneratorManager.PlaceSupply(new Vector3(worldPosition.x, worldPosition.y, pos.z));
				if (success)
				{
					suppliesMap.SetTile(pos, EmptyTile);
				}
			}
		}

		// Right-click -> delete tower
		if (Input.GetMouseButtonUp(1) && !GameState.Instance.IsEditing)
		{

			var top = GetTopTile(mousePos);
			Vector3Int pos = top.First;
			pos.z += 1;

			var tile = towersMap.GetTile(pos);
			if (tile != null)
			{
				Vector3 worldPosition = towersMap.CellToWorld(pos);
				TowerManager.RemoveTower(new Vector3(worldPosition.x, worldPosition.y, pos.z));
				towersMap.SetTile(pos, null);
			}

		}
	}

	Pair<Vector3Int, bool> GetTopTile(Vector3Int gridCell)
	{
		bool isHighTile = false;
		for (int z = 100; z >= 0; z--)
		{
			gridCell.z = z;
			var tile = baseMap.GetTile(new Vector3Int(gridCell.x, gridCell.y, z));
			if (tile == null || tile.name == null)
			{
			}
			else
			{
				isHighTile = tile.name == "tiles_19";
				return new Pair<Vector3Int, bool>(gridCell, isHighTile);
			}

		}
		return new Pair<Vector3Int, bool>(gridCell, isHighTile);
	}


	public static Vector3Int GetMousePosition(Grid grid, Tilemap tilemap)
	{
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		Vector3Int gridCell = tilemap.WorldToCell(mouseWorldPos);
		//gridCell.x += gridCell.z;
		//gridCell.y += gridCell.z;
		//gridCell.z = 0;

		// Reference:
		// https://answers.unity.com/questions/1622564/selecting-a-tile-with-z-as-y-isometric-tilemaps.html
		// The problem will keep happening because there are different heights of tiles
		// Another case to take into consideration is the bottom most tile of the grid

		//mouseWorldPos.y -= 1.44f;
		//for (int z = 10; z >= 0; z--)
		//{
		//	gridCell = tilemap.WorldToCell(mouseWorldPos);
		//	var tile = tilemap.GetTile(new Vector3Int(gridCell.x, gridCell.y, z));

		//	if (tile != null)
		//	{
		//		break;
		//	}
		//	mouseWorldPos.y += 0.16f;
		//}

		gridCell.x += gridCell.z;
		gridCell.y += gridCell.z;
		gridCell.z = 0;
		return gridCell;
	}
}
