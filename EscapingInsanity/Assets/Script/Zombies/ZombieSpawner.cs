
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Zombie Settings")]
    public GameObject zombiePrefab;
    public Transform player;

    [Header("Spawn Settings")]
    public int startingZombies = 5;
    public float spawnRadius = 20f;
    public float minimumDistance = 5f;

    [Header("Spawn Timing")]
    public float spawnInterval = 1.5f;
    public float spawnRetryDelay = 1f;

    [Header("NavMesh Settings")]
    public float navMeshSearchDistance = 3f;
    public int spawnAttempts = 20;

    private void Start()
    {
        StartCoroutine(SpawnZombiesOverTime());
    }

    private IEnumerator SpawnZombiesOverTime()
    {
        // Keep searching for the player if it isn't assigned yet.
        while (player == null)
        {
            GameObject target =
                GameObject.FindGameObjectWithTag("Player");

            if (target != null)
                player = target.transform;

            if (player == null)
                yield return new WaitForSeconds(spawnRetryDelay);
        }

        // Spawn the initial group gradually.
        for (int i = 0; i < startingZombies; i++)
        {
            while (!SpawnZombie())
            {
                yield return new WaitForSeconds(spawnRetryDelay);
            }

            yield return new WaitForSeconds(0.5f);
        }

        // Continue spawning forever.
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (player == null)
                continue;

            // Retry until one zombie successfully spawns.
            while (!SpawnZombie())
            {
                yield return new WaitForSeconds(spawnRetryDelay);

                if (player == null)
                    break;
            }
        }
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

            if (!NavMesh.SamplePosition(
                candidatePosition,
                out NavMeshHit hit,
                navMeshSearchDistance,
                NavMesh.AllAreas))
            {
                continue;
            }

            GameObject zombie = Instantiate(
                zombiePrefab,
                hit.position,
                Quaternion.identity
            );

            ZombieAI ai =
                zombie.GetComponentInChildren<ZombieAI>();

            ZombieHealth health =
                zombie.GetComponent<ZombieHealth>();

            NavMeshAgent agent =
                zombie.GetComponent<NavMeshAgent>();

            // Verify that the zombie can function.
            if (ai == null || health == null ||
                agent == null || !agent.enabled ||
                !agent.isOnNavMesh)
            {
                Destroy(zombie);
                continue;
            }

            // Assign the player to the zombie AI.
            ai.player = player;

            return true;
        }

        return false;
    }

    public void ZombieDefeated()
    {
        // No immediate replacement.
        // The spawning coroutine handles future spawns.
    }
}
