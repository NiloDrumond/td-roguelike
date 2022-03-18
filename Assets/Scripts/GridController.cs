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
	[SerializeField] private PathController pathController = null;
    [SerializeField] private IsometricRuleTile hoverTile = null;

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
			var pos = GetTopTile(mousePos);
			interactiveMap.SetTile(previousCursorPosition, null); // Remove old hoverTile
            interactiveMap.SetTile(pos, hoverTile);
            previousCursorPosition = pos;
        }

		// Left-click
		if(Input.GetMouseButtonUp(0))
		{
			pathController.AddWaypoint(mousePos);
		}

		// Right-click
		if(Input.GetMouseButtonUp(1))
		{
			pathController.RemoveWaypoint(mousePos);
		}
    }

	Vector3Int GetTopTile(Vector3Int gridCell)
	{
		for (int z = 1; z < 100; z++)
		{
			var tile = baseMap.GetTile(new Vector3Int(gridCell.x, gridCell.y, z));
			if(tile == null)
			{
				break;
			}
			gridCell.z = z;
		}
		return gridCell;
	}



	// https://answers.unity.com/questions/1622564/selecting-a-tile-with-z-as-y-isometric-tilemaps.html
	//private void OnMouseDown()
	//{
	//	// Get mouseClick using new input system (you can also use the old version of previous answers)
	//	Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	//	mouseWorldPos.y -= 0.08f;
	//	Vector3Int gridCell = grid.WorldToCell(mouseWorldPos);
	//	Debug.Log(gridCell);
	//	return;
	//	// Using tilemap instead works aswell
	//	// Vector3Int tileCell = tilemap.WorldToCell(mousePos);

	//	//Debug.Log("mousePos : " + mouseWorldPos + " cellPos_0  : " + gridCell);


	//	// You can use that loop to get all tiles of current pile of tiles,
	//	// starting from the grid cell (with z = 0) that you clicked.
	//	//for (int z = 10; z > -1; z--)
	//	//{
	//	//	gridCell.z = z;
	//	//	var tile = baseMap.GetTile(gridCell);
	//	//	if (tile != null)
	//	//	{
	//	//		Debug.Log("grid" + gridCell + " found " + tile.name);
	//	//		// If you need only the top most tile of that pile

	//	//	}
	//	//}

	//	// You can use this loop to check all the tiles that could be 
	//	// hiding the grid cell with (z = 0) that you clicked.
	//	// We start from the front most tile which with an elevation of 9
	//	// and would correspond to a 1.44 below current y
	//	// and then we decrease z as we increase y by 0.16f (half cell height)
	//	mouseWorldPos.y -= 0.80f; // to be adapted depending on your max Z and the start of the loop below
	//	for (int z = 10; z > -1; z--)
	//	{
	//		gridCell = grid.WorldToCell(mouseWorldPos);
	//		gridCell.z = z;
	//		var tile = baseMap.GetTile(gridCell);
	//		if (tile != null && gridCell.z == z)
	//		{
	//			Debug.Log("grid Z" + gridCell + " found " + tile.name);

	//			// If you need only the first tile encountered

	//		}
	//		mouseWorldPos.y += 0.08f;
	//	}
	//}



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