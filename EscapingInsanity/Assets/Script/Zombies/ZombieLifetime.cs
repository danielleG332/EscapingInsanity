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

}
