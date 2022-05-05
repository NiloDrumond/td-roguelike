using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private PixelPerfectCamera pixelCam;
    [SerializeField] private Tilemap tilemap; 

    [SerializeField] private float zoomStep, minCamSize, maxCamSize;


	private Vector3 dragOrigin;
    private bool isDragging = false;

    private float mapMinX, mapMaxX, mapMinY, mapMaxY;



	// Start is called before the first frame update
	void Awake()
    {
        tilemap.CompressBounds();

        mapMinX = tilemap.transform.position.x - tilemap.localBounds.size.x / 2;
        mapMaxX = tilemap.transform.position.x + tilemap.localBounds.size.x / 2;

        mapMinY = tilemap.transform.position.y - tilemap.localBounds.size.y;
        mapMaxY = tilemap.transform.position.y + tilemap.localBounds.size.y / 2;
        Debug.Log(tilemap.transform.position);
        Debug.Log(tilemap.localBounds.size);
    }

    // Update is called once per frame
    void Update()
    {
        PanCamera();
        Zoom();
    }

    private void PanCamera()
	{
        if(Input.GetMouseButtonUp(1))
		{
            isDragging = false;
        }

        if (InputManager.IsPointerOverUIObject()) return;


        if(Input.GetMouseButtonDown(1))
		{
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;

        }

        if(Input.GetMouseButton(1) && isDragging)
		{
            Vector3 diff = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);

            cam.transform.position = ClampCamera(cam.transform.position + diff);
        }
    }

    private void Zoom()
    {

        if(Input.mouseScrollDelta.y > 0f) // Scroll up
        {
            float newSize = pixelCam.assetsPPU + zoomStep;

            pixelCam.assetsPPU = (int)Mathf.Clamp(newSize, minCamSize, maxCamSize);
        }

        if(Input.mouseScrollDelta.y < 0f) // Scroll down
        {
            float newSize = pixelCam.assetsPPU - zoomStep;

            pixelCam.assetsPPU = (int)Mathf.Clamp(newSize, minCamSize, maxCamSize);
        }

        cam.transform.position = ClampCamera(cam.transform.position);
    }
    
    private Vector3 ClampCamera(Vector3 targetPosition)
	{
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;

        float minY = mapMinY + camHeight;
        float maxY = mapMaxY - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, targetPosition.z);
	}

}
