using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieLifetime : MonoBehaviour
{
    private ZombieSpawner spawner;
    private bool defeated;

    public void Initialize(ZombieSpawner owner)
    {
        spawner = owner;
    }

    public void Defeat()
    {
        if (defeated)
            return;

        defeated = true;

        if (spawner != null)
            spawner.ZombieDefeated();

        Destroy(gameObject);
    }
}
