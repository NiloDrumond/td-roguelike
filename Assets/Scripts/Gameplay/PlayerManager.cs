using System.Collections;
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static float Health;
	public static float MaxHealth = 100;

	public static Supplies Supplies;

	private static bool isInitialized = false;

	private static ProgressBar healthBar;
	private static TMP_Text mineralsCount;

	public static void Init()
	{
		if (!isInitialized)
		{
			Health = MaxHealth;
			healthBar = GameObject.Find("UI Canvas/HealthBar").GetComponent<ProgressBar>();
			healthBar.speed = 0;
			UpdateHealthBar();

			mineralsCount = GameObject.Find("UI Canvas/Supplies/Minerals Value").GetComponent<TMP_Text>();
			isInitialized = true;

            Supplies = new Supplies
            {
                { Supply.Mineral, 50 }
            };
			UpdateSupplies();

		}
		else
		{
			Debug.LogWarning("ENTITYSUMMONER: this class has already initialized");
		}
	}

	private static void UpdateSupplies()
	{
		mineralsCount.text = Supplies[Supply.Mineral].ToString();
	}

	public static void AddSupplies(Supplies supplies)
	{
		Supplies += supplies;
		UpdateSupplies();
	}

	public static bool SpendSupplies(Supplies cost) 
	{
		if (!Supplies.CanSubtract(cost)) return false;
		Supplies -= cost;
		UpdateSupplies();
		return true;
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
			GameLoopManager.Instance.IsRunning = false;
		}
	}
}
