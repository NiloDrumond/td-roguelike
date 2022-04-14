using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorBehaviour : MonoBehaviour {
    public float Lifetime;
    public float MaxLifetime;
    public float Interval;

    [Space]
    [Header("Production")]
    public int MineralProduction;


    private readonly Supplies production = new Supplies();
    private float delay;

    public void Init() {
        delay = Interval;
        production.Add(Supply.Mineral, MineralProduction);
    }

    public void Tick()
	{
        if (delay > 0f)
        {
            delay -= Time.deltaTime;
            return;
        }
        PlayerManager.AddSupplies(production);
        delay = Interval;
    }

}