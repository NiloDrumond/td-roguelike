using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GridController : MonoBehaviour
{
    [SerializeField] private Grid grid = null;
    [SerializeField] private Tilemap layer1 = null;
    [SerializeField] private Tilemap layer2 = null;
    [SerializeField] private Tilemap layer3 = null;
    [SerializeField] private Tilemap interactiveMap = null;
    [SerializeField] private Tilemap towersMap = null;
    [SerializeField] private Tilemap suppliesMap = null;
    [SerializeField] private IsometricRuleTile hoverTile = null;
    [SerializeField] private Tile EmptyTile = null;

    private Vector3Int previousCursorPosition = new Vector3Int();


    private bool CanPlaceTower(Vector3Int cell) 
    {
        //cell.x += 1;
        //cell.y += 1;
        TileBase tile = layer3.GetTile(cell);
        if(tile == null)
        {
            tile = layer2.GetTile(cell);
            if(tile == null)
            {
                tile = layer1.GetTile(cell);
                if(tile == null)
                {
                    return false;
                }
            }
        }
        Debug.Log(tile.name);
        return false;
    }

    private void OnMouseOver()
    {
        if (InputManager.IsPointerOverUIObject())
        {
            return;
        }
        RegionsManager.DisableHighlight();
       
        Vector3Int mousePos = GetCellAtMouse();
        //if (GameState.Instance.IsUnlockingRegion)
        //{
        //    interactiveMap.SetTile(previousCursorPosition, null);
        //    int region = RegionsManager.GetUnlockableRegion(mousePos);
        //    if (region >= 0)
        //    {
        //        if (Input.GetMouseButtonUp(0))
        //        {
        //            RegionsManager.ActivateRegion(region);
        //        }
        //        RegionsManager.HighlightRegion(region);
        //    }
        //    return;
        //}

        // Mouse over -> highlight tile
        if (!mousePos.Equals(previousCursorPosition))
        {           
            var top = GetTopTile(mousePos);
            // Debug.Log(top);
            Vector3Int high = new Vector3Int(mousePos.x - 100, mousePos.y - 100, mousePos.z + 100);
            interactiveMap.SetTile(previousCursorPosition, null); // Remove old hoverTile
            interactiveMap.SetTile(high, hoverTile);
            previousCursorPosition = high;
            
        }

        // Left-click -> upgrade tower
        if (Input.GetMouseButtonUp(0) && !GameState.Instance.IsEditing && GameState.Instance.IsUpgrading)
        {
            var top = GetTopTile(mousePos);
            top.z += 1;
            var tile = towersMap.GetTile(top);

            if (tile != null)
            {
                Vector3 worldPosition = towersMap.CellToWorld(top);
                bool activatedRegion = RegionsManager.CheckForPlacement(mousePos);
                if (activatedRegion)
                {
                    bool enoughResources = TowerManager.UpgradeTower(new Vector3(worldPosition.x, worldPosition.y, top.z));
                    if (enoughResources)
                    {
                        //usar isso pra mudar o tile?
                        towersMap.SetTile(top, EmptyTile);
                    }
                }

            }
        }
        // Left-click -> place tower
        if (Input.GetMouseButtonUp(0) && !GameState.Instance.IsEditing)
        {
            var top = GetTopTile(mousePos);
            top.z += 1;
            var tile = towersMap.GetTile(top);
            if (tile == null)
            {
                CanPlaceTower(mousePos);
                Vector3 worldPosition = towersMap.CellToWorld(top);
                bool activatedRegion = RegionsManager.CheckForPlacement(mousePos);
                if (activatedRegion)
                {
                    bool enoughResources = TowerManager.PlaceTower(new Vector3(worldPosition.x, worldPosition.y, top.z));
                    if (enoughResources)
                    {
                        towersMap.SetTile(top, EmptyTile);
                    }
                }

            }
        }

        // Middle-click -> place supply
        if (Input.GetMouseButtonUp(2) && !GameState.Instance.IsEditing)
        {
            var top = GetTopTile(mousePos);
            top.z += 1;
            var tile = suppliesMap.GetTile(top);
            if (tile == null)
            {
                Vector3 worldPosition = suppliesMap.CellToWorld(top);
                bool success = GeneratorManager.PlaceSupply(new Vector3(worldPosition.x, worldPosition.y, top.z));
                if (success)
                {
                    suppliesMap.SetTile(top, EmptyTile);
                }
            }
        }

        // Right-click -> delete tower
        if (Input.GetMouseButtonUp(1) && !GameState.Instance.IsEditing)
        {

            var top = GetTopTile(mousePos);
            top.z += 1;

            var tile = towersMap.GetTile(top);
            if (tile != null)
            {
                Vector3 worldPosition = towersMap.CellToWorld(top);
                TowerManager.RemoveTower(new Vector3(worldPosition.x, worldPosition.y, top.z));
                towersMap.SetTile(top, null);
            }

        }
    }

    Vector3Int GetTopTile(Vector3Int gridCell)
    {
        Vector3Int cell;
        TileBase tile;
        cell = new Vector3Int(gridCell.x - 2, gridCell.y - 2, gridCell.z);
        tile = layer3.GetTile(cell);
        if (tile != null)
        {
            return cell;
        }
        cell = new Vector3Int(gridCell.x - 1, gridCell.y - 1, gridCell.z);
        tile = layer2.GetTile(cell);
        if (tile != null)
        {
            return cell;
        }
        return gridCell;
    }


    public Vector3Int GetCellAtMouse()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.y += 0.08f;
        mouseWorldPos.z = 0;
        Vector3Int gridCell = layer1.WorldToCell(mouseWorldPos);

        var tile = layer3.GetTile(gridCell);
        if(tile != null)
        {
            gridCell.x -= 2;
            gridCell.y -= 2;
            gridCell.z = 2;
            return gridCell;
        }
        tile = layer2.GetTile(gridCell);
        if (tile != null)
        {
            gridCell.x -= 1;
            gridCell.y -= 1;
            gridCell.z = 1;
            return gridCell;
        }
        return gridCell;
    }

    public static Vector3Int GetMousePosition(Grid grid)
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.y += 0.08f;
        var gridCell = grid.WorldToCell(mouseWorldPos);

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
