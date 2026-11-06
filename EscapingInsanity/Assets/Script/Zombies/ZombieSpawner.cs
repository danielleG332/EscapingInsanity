
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Zombie Settings")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private Transform player;

    [Header("Spawn Settings")]
    [SerializeField] private int startingZombies = 5;
    [SerializeField] private float spawnInterval = 5f;

    [Header("Spawn Distance")]
    [SerializeField] private float spawnRadius = 20f;
    [SerializeField] private float minimumDistance = 5f;

    [Header("NavMesh Settings")]
    [SerializeField] private float navMeshSearchDistance = 3f;
    [SerializeField] private int spawnAttempts = 20;

    private void Start()
    {
        if (player == null)
        {
            GameObject target =
                GameObject.FindGameObjectWithTag("Player");

            if (target != null)
                player = target.transform;
        }

        if (zombiePrefab == null)
        {
            Debug.LogError("ZombieSpawner: Missing zombie prefab!");
            return;
        }

        if (player == null)
        {
            Debug.LogError("ZombieSpawner: Player not found!");
            return;
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        // Spawn the starting zombies.
        for (int i = 0; i < startingZombies; i++)
        {
            if (SpawnZombie())
            {
                Debug.Log("Starting zombie spawned: " + (i + 1));
            }
            else
            {
                Debug.LogWarning(
                    "Could not spawn starting zombie: " + (i + 1)
                );
            }

            yield return new WaitForSeconds(0.5f);
        }

        // Keep spawning forever.
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (player == null)
                continue;

            if (SpawnZombie())
            {
                Debug.Log("A new zombie spawned!");
            }
            else
            {
                Debug.LogWarning(
                    "Zombie spawn failed. Will try again."
                );
            }
        }
    }

    private bool SpawnZombie()
    {
        for (int i = 0; i < spawnAttempts; i++)
        {
            Vector2 direction =
                Random.insideUnitCircle.normalized;

            if (direction.sqrMagnitude < 0.001f)
                direction = Vector2.right;

            float distance = Random.Range(
                minimumDistance,
                spawnRadius
            );

            Vector3 candidatePosition = player.position +
                new Vector3(
                    direction.x * distance,
                    0f,
                    direction.y * distance
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

            if (ai == null || health == null || agent == null)
            {
                Debug.LogError(
                    "Zombie prefab is missing a required component!"
                );

                Destroy(zombie);
                continue;
            }

            if (!agent.enabled || !agent.Warp(hit.position))
            {
                Debug.LogError(
                    "Zombie could not be placed on the NavMesh!"
                );

                Destroy(zombie);
                continue;
            }

            ai.player = player;

            return true;
        }

        return false;
    }
}
