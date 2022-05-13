using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    [SerializeField] private TMP_Text unlockRegionText;
    [SerializeField] private TMP_Text regionsUnlockedText;
    public bool IsEditing;
    private bool isUnlockingRegion;
    public bool IsUnlockingRegion
    {
        get { return isUnlockingRegion; }
        set
        {
            unlockRegionText.enabled = value;
            isUnlockingRegion = value;
        }
    }

    public bool IsUpgrading;
    private int unlockedRegions = 0;
    public int UnlockedRegions
    {
        get { return unlockedRegions; }
        set
        {
            regionsUnlockedText.text = $"{value}/9";
            unlockedRegions = value;
        }
    }
    public bool AllRegionsUnlocked = false;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

}
