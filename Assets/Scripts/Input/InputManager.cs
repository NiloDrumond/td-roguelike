using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
	private static GameObject ui;

	public static void Init()
	{
		ui = GameObject.Find("UI Canvas");
	}

	public static bool IsPointerOverUIObject()
	{
		// get current pointer position and raycast it
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

		// check if the target is in the UI
		foreach (RaycastResult r in results)
		{
			bool isUIClick = r.gameObject.transform.IsChildOf(ui.transform);
			if (isUIClick)
			{
				return true;
			}
		}
		return false;
	}
}
