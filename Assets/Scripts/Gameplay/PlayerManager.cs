using System.Collections;
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static float Health;
	public static float MaxHealth = 100;

	private static bool isInitialized = false;

	[SerializeField] private static ProgressBar healthBar;

	public static void Init()
	{
		if (!isInitialized)
		{
			Health = MaxHealth;
			healthBar = GameObject.Find("UI Canvas/HealthBar").GetComponent<ProgressBar>();
			healthBar.speed = 0;

			isInitialized = true;
		}
		else
		{
			Debug.LogWarning("ENTITYSUMMONER: this class has already initialized");
		}
	}

	private static void UpdateHealthBar()
	{
		healthBar.currentPercent = (Mathf.Max(Health, 0) / MaxHealth) * 100f;

	}

	public static void ReceiveDamage(float damage)
	{
		Health -= damage;
		UpdateHealthBar();
		if (Health <= 0)
		{
			GameLoopManager.LoopShouldEnd = true;
		}
	}
}
