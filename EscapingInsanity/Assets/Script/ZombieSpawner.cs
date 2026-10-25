using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public Transform player;

    public int maxZombies = 5;
    public float spawnRadius = 12f;
    public float minimumDistance = 5f;

    private int aliveZombies = 0;

    void Start()
    {
        if (player == null)
        {
            GameObject target = GameObject.FindGameObjectWithTag("Player");

            if (target != null)
                player = target.transform;
        }

        if (zombiePrefab == null || player == null)
        {
            Debug.LogError("Assign the zombie prefab and player!");
            return;
        }

        for (int i = 0; i < maxZombies; i++)
        {
            SpawnZombie();
        }
    }

    public void SpawnZombie()
    {
        if (aliveZombies >= maxZombies ||
            zombiePrefab == null || player == null)
            return;

        for (int attempt = 0; attempt < 20; attempt++)
        {
            Vector2 random = Random.insideUnitCircle.normalized
                * Random.Range(minimumDistance, spawnRadius);

            Vector3 candidate = player.position
                + new Vector3(random.x, 0f, random.y);

            if (NavMesh.SamplePosition(
                candidate, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                GameObject zombie = Instantiate(
                    zombiePrefab, hit.position, Quaternion.identity);

                ZombieAI ai = zombie.GetComponent<ZombieAI>();

                if (ai != null)
                {
                    ai.player = player;
                    aliveZombies++;

                    // Called when this zombie is defeated.
                    ZombieLifetime lifetime =
                        zombie.GetComponent<ZombieLifetime>();

                    if (lifetime != null)
                        lifetime.Initialize(this);
                    else
                        Debug.LogWarning(
                            "Zombie prefab needs ZombieLifetime.");
                }
                else
                {
                    Destroy(zombie);
                    Debug.LogError("Zombie prefab needs ZombieAI.");
                }

                return;
            }
        }

        Debug.LogWarning("Could not find a valid zombie spawn position.");
    }

    public void ZombieDefeated()
    {
        aliveZombies = Mathf.Max(0, aliveZombies - 1);
        SpawnZombie();
    }
}
