using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IPointerClickHandler
{
    private Button button;
    private ButtonManager buttonManager;
	// Start is called before the first frame update

	private void Start()
	{
        button = GetComponent<Button>();
        buttonManager = GetComponent<ButtonManager>();
    }

    // Maybe useful?
    public void OnPointerClick(PointerEventData eventData)
    {
        //if (eventData.button == PointerEventData.InputButton.Left)
        //    Debug.Log("Left click");
        //else if (eventData.button == PointerEventData.InputButton.Middle)
        //    Debug.Log("Middle click");
        //else if (eventData.button == PointerEventData.InputButton.Right)
        //    Debug.Log("Right click");
    }

}
