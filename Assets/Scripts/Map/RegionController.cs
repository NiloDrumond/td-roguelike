using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RegionController : MonoBehaviour
{
    [SerializeField] private TilemapRenderer tilemapRenderer;
    public bool Activated = false;


    public void ToggleHightlight(bool on)
    {
        tilemapRenderer.enabled = on;
    }

}
