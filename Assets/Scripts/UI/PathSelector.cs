using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;
using UnityEngine.Events;

public class PathSelector : MonoBehaviour
{
    [SerializeField] private HorizontalSelector selector;

    public void Start()
	{
        if(!GameState.Instance.IsEditing)
		{
            gameObject.SetActive(false);
		}
        selector.enableIndicators = false;
        selector.enableIcon = false;
    }

    public void RegisterPaths(string[] arr, UnityAction<int> onChange) 
    {

        // Clean old paths
        var list = selector.itemList;
        foreach(var item in list)
		{
            selector.RemoveItem(item.itemTitle);
		}

		for (int i = 0; i < arr.Length; i++)
		{
            selector.CreateNewItem(arr[i]);
		}

        selector.onValueChanged.AddListener(onChange);

        selector.SetupSelector();
        selector.index = 0;
        selector.UpdateUI();
    }
}
