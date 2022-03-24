using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GridController : MonoBehaviour
{

	[SerializeField] private bool isEditing;
	[Space]
	private Grid grid;
    [SerializeField] private Tilemap baseMap = null;
    [SerializeField] private Tilemap interactiveMap = null;
	[SerializeField] private Tilemap towersMap = null;
	[SerializeField] private PathController pathController = null;
    [SerializeField] private IsometricRuleTile hoverTile = null;
	[SerializeField] private IsometricRuleTile highHoverTile = null;
	[SerializeField] private Tile EmptyTile = null;


	private Vector3Int previousCursorPosition = new Vector3Int();

    // Start is called before the first frame update
    void Start()
    {
        grid = gameObject.GetComponent<Grid>();
		pathController = gameObject.transform.Find("Path").GetComponent<PathController>();
	}



    // Update is called once per frame
    void Update()
    {
        // Mouse over -> highlight tile
        Vector3Int mousePos = GetMousePosition();
        if (!mousePos.Equals(previousCursorPosition))
		{
			var top = GetTopTile(mousePos);
			Vector3Int pos = top.First;
			bool isHigh = top.Second;
			interactiveMap.SetTile(previousCursorPosition, null); // Remove old hoverTile
            interactiveMap.SetTile(pos, isHigh ? highHoverTile : hoverTile);
            previousCursorPosition = pos;
        }

		
    }

	

	private void OnMouseOver()
	{
		if(InputManager.IsPointerOverUIObject())
		{
			return;
		}
		// Left-click
		if (Input.GetMouseButtonUp(0))
		{
			Vector3Int mousePos = GetMousePosition();
			if (isEditing)
			{
				pathController.AddWaypoint(mousePos);
			}
			else
			{
				var top = GetTopTile(mousePos);
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

		}

		// Right-click
		if (Input.GetMouseButtonUp(1))
		{
			Vector3Int mousePos = GetMousePosition();
			if (isEditing)
			{
				pathController.RemoveWaypoint(mousePos);
			}
			else
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
			} else
			{
				isHighTile = tile.name == "tiles_19";
				return new Pair<Vector3Int, bool>(gridCell, isHighTile);
			}
			
		}
		return new Pair<Vector3Int, bool>(gridCell, isHighTile);
	}


	Vector3Int GetMousePosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mouseWorldPos.y -= 0.08f;

		Vector3Int gridCell = grid.WorldToCell(mouseWorldPos);
		gridCell.x += gridCell.z;
		gridCell.y += gridCell.z;
		gridCell.z = 0;


		// Reference:
		// https://answers.unity.com/questions/1622564/selecting-a-tile-with-z-as-y-isometric-tilemaps.html
		// The problem will keep happening because there are different heights of tiles

		mouseWorldPos.y -= 0.8f;
		for (int z = 10; z > -1; z--)
		{
			gridCell = grid.WorldToCell(mouseWorldPos);
			var tile = baseMap.GetTile(new Vector3Int(gridCell.x, gridCell.y, z));
			
			if (tile != null)
			{
				break;
			}
			mouseWorldPos.y += 0.08f;
		}

		gridCell.x += gridCell.z;
		gridCell.y += gridCell.z;
		gridCell.z = 0;
		return gridCell;
	}
}
