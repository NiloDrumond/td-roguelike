using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GridController : MonoBehaviour
{
    private Grid grid;
    [SerializeField] private Tilemap baseMap = null;
    [SerializeField] private Tilemap interactiveMap = null;
	[SerializeField] private Tilemap towersMap = null;
	[SerializeField] private PathController pathController = null;
    [SerializeField] private IsometricRuleTile hoverTile = null;
	[SerializeField] private IsometricRuleTile highHoverTile = null;

	[SerializeField] private Path path = null;


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

		// Left-click
		if(Input.GetMouseButtonUp(0))
		{
			var top = GetTopTile(mousePos);
			Vector3Int pos = top.First;
			pos.z += 1;
			var tile = towersMap.GetTile(pos);
			if(tile == null)
			{
				var tower = TowerManager.PlaceTower(pos, towersMap.CellToWorld(pos));
				if(tower != null)
				{
					towersMap.SetTile(pos, tower.Data.Tile);
				}
			}
			// pathController.AddWaypoint(mousePos);
		}

		// Right-click
		if(Input.GetMouseButtonUp(1))
		{
			var top = GetTopTile(mousePos);
			Vector3Int pos = top.First;
			pos.z += 1;
			
			var tower = TowerManager.RemoveTower(pos);
			if (tower != null)
			{
				towersMap.SetTile(pos, null);
			}
			
			// pathController.RemoveWaypoint(mousePos);
		}
    }

	Pair<Vector3Int, bool> GetTopTile(Vector3Int gridCell)
	{
		bool isHighTile = false;
		for (int z = 1; z < 100; z++)
		{
			var tile = baseMap.GetTile(new Vector3Int(gridCell.x, gridCell.y, z));
			if(tile == null)
			{
				break;
			} else
			{
				isHighTile = tile.name == "tiles_19";
			}
			gridCell.z = z;
		}
		return new Pair<Vector3Int, bool>(gridCell, isHighTile);
	}


	Vector3Int GetMousePosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mouseWorldPos.y -= 0.08f;
        Vector3Int mousePos = baseMap.WorldToCell(mouseWorldPos);
		mousePos.x += mousePos.z;
		mousePos.y += mousePos.z;
		mousePos.z = 0;
		return mousePos;
	}
}

// Reference:
// https://answers.unity.com/questions/1622564/selecting-a-tile-with-z-as-y-isometric-tilemaps.html