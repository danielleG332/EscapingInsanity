using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Zombie Settings")]
    public GameObject zombiePrefab;
    public Transform player;

    [Header("Spawn Settings")]
    public int maxZombies = 14;
    public float spawnRadius = 12f;
    public float minimumDistance = 5f;

    [Header("Spawn Retry")]
    public float spawnRetryDelay = 1f;

    private int aliveZombies = 0;

    void Start()
    {
        if (player == null)
        {
            GameObject target =
                GameObject.FindGameObjectWithTag("Player");

            if (target != null)
                player = target.transform;
        }

        if (zombiePrefab == null || player == null)
            return;

        StartCoroutine(MaintainZombieCount());
    }

    IEnumerator MaintainZombieCount()
    {
        while (true)
        {
            if (aliveZombies < maxZombies)
            {
                bool spawned = SpawnZombie();

                if (!spawned)
                    yield return new WaitForSeconds(spawnRetryDelay);
                else
                    yield return null;
            }
            else
            {
                yield return null;
            }
        }
    }

    bool SpawnZombie()
    {
        if (aliveZombies >= maxZombies ||
            zombiePrefab == null || player == null)
            return false;

        for (int attempt = 0; attempt < 20; attempt++)
        {
            Vector2 random =
                Random.insideUnitCircle.normalized *
                Random.Range(minimumDistance, spawnRadius);

            Vector3 candidate = player.position +
                new Vector3(random.x, 0f, random.y);

            if (NavMesh.SamplePosition(
                candidate,
                out NavMeshHit hit,
                3f,
                NavMesh.AllAreas))
            {
                GameObject zombie = Instantiate(
                    zombiePrefab,
                    hit.position,
                    Quaternion.identity
                );

                ZombieAI ai = zombie.GetComponent<ZombieAI>();
                ZombieHealth health =
                    zombie.GetComponent<ZombieHealth>();

                if (ai == null || health == null)
                {
                    Destroy(zombie);
                    return false;
                }

                ai.player = player;
                health.SetSpawner(this);

                aliveZombies++;

                return true;
            }
        }

        return false;
    }

    public void ZombieDefeated()
    {
        aliveZombies = Mathf.Max(0, aliveZombies - 1);
    }
}
