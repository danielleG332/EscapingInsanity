
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

    [Header("Spawn Timing")]
    public float spawnInterval = 0.5f;
    public float spawnRetryDelay = 1f;

    [Header("NavMesh Settings")]
    public float navMeshSearchDistance = 3f;
    public int spawnAttempts = 20;

    private Coroutine spawnCoroutine;

    private void Start()
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

        spawnCoroutine = StartCoroutine(MaintainZombieCount());
    }

    private IEnumerator MaintainZombieCount()
    {
        while (player != null)
        {
            int currentZombies = CountLivingZombies();

            if (currentZombies < maxZombies)
            {
                bool spawned = SpawnZombie();

                if (!spawned)
                {
                    yield return new WaitForSeconds(spawnRetryDelay);
                    continue;
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    [System.Obsolete]

    private int CountLivingZombies()
    {
        ZombieHealth[] zombies =
            FindObjectsOfType<ZombieHealth>();

        int count = 0;

        foreach (ZombieHealth health in zombies)
        {
            if (health == null || health.IsDead)
                continue;

            ZombieAI ai =
                health.GetComponentInChildren<ZombieAI>();

            NavMeshAgent agent =
                health.GetComponentInChildren<NavMeshAgent>();

            // Remove zombies that cannot function.
            if (ai == null || !ai.enabled ||
                agent == null || !agent.enabled ||
                !agent.isOnNavMesh)
            {
                Destroy(health.gameObject);
                continue;
            }

            count++;
        }

        return count;
    }


    private bool SpawnZombie()
    {
        if (zombiePrefab == null || player == null)
            return false;

        for (int i = 0; i < spawnAttempts; i++)
        {
            Vector2 randomDirection =
                Random.insideUnitCircle.normalized;

            if (randomDirection.sqrMagnitude < 0.001f)
                randomDirection = Vector2.right;

            float distance = Random.Range(
                minimumDistance,
                spawnRadius
            );

            Vector3 candidatePosition = player.position +
                new Vector3(
                    randomDirection.x * distance,
                    0f,
                    randomDirection.y * distance
                );

            if (NavMesh.SamplePosition(
                candidatePosition,
                out NavMeshHit hit,
                navMeshSearchDistance,
                NavMesh.AllAreas))
            {
                GameObject zombie = Instantiate(
                    zombiePrefab,
                    hit.position,
                    Quaternion.identity
                );

                ZombieAI ai = zombie.GetComponentInChildren<ZombieAI>();
                ZombieHealth health = zombie.GetComponent<ZombieHealth>();

                if (ai == null || health == null)
                {
                    Destroy(zombie);
                    return false;
                }

                ai.player = player;
                

                return true;
            }
        }

        return false;
    }

    // Kept for compatibility with your existing ZombieHealth script.
    // The spawner now counts living zombies directly.
    public void ZombieDefeated()
    {
    }
}
