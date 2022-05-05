using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    [SerializeField] private TMP_Text unlockRegionText;
    public bool IsEditing;
    private bool isUnlockingRegion;
    public bool IsUnlockingRegion;
    public bool IsUpgrading
    {
        get { return isUnlockingRegion; }
        set
        {
            unlockRegionText.enabled = value;
            isUnlockingRegion = value;
        }
    }
    public int UnlockedRegions = 0;
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
