
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public string enemyName;
        public GameObject enemyPrefab;

        [Min(1)]
        public int unlockLevel = 1;

        [Min(0.01f)]
        public float spawnWeight = 1f;
    }

    [Header("Enemy Types")]
    public List<EnemyType> enemyTypes =
        new List<EnemyType>();

    [Header("Player Settings")]
    public Transform player;

    private XPManager xpManager;

    [Header("Spawn Settings")]
    public int startingZombies = 5;
    public float spawnRadius = 20f;
    public float minimumDistance = 5f;

    [Header("Spawn Timing")]
    public float spawnInterval = 5f;
    public float spawnRetryDelay = 1f;

    [Header("NavMesh Settings")]
    public float navMeshSearchDistance = 3f;
    public int spawnAttempts = 20;

    private void Start()
    {
        if (player == null)
        {
            GameObject target =
                GameObject.FindGameObjectWithTag("Player");

            if (target != null)
                player = target.transform;
        }

        if (player == null)
        {
            Debug.LogError("ZombieSpawner: Player not found!");
            return;
        }

        xpManager = player.GetComponent<XPManager>();

        if (xpManager == null)
            xpManager = player.GetComponentInChildren<XPManager>();

        if (xpManager == null)
        {
            Debug.LogError("ZombieSpawner: XPManager not found!");
            return;
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        // Spawn the initial group gradually.
        for (int i = 0; i < startingZombies; i++)
        {
            while (!SpawnZombie())
            {
                yield return new WaitForSeconds(spawnRetryDelay);
            }

            yield return new WaitForSeconds(0.5f);
        }

        // Continue spawning indefinitely.
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            while (!SpawnZombie())
            {
                yield return new WaitForSeconds(spawnRetryDelay);
            }
        }
    }

    private bool SpawnZombie()
    {
        if (player == null || xpManager == null)
            return false;

        EnemyType selectedEnemy = SelectEnemy();

        if (selectedEnemy == null ||
            selectedEnemy.enemyPrefab == null)
            return false;

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
                continue;

            GameObject zombie = Instantiate(
                selectedEnemy.enemyPrefab,
                hit.position,
                Quaternion.identity
            );

            ZombieAI ai =
                zombie.GetComponentInChildren<ZombieAI>();

            ZombieHealth health =
                zombie.GetComponent<ZombieHealth>();

            NavMeshAgent agent =
                zombie.GetComponent<NavMeshAgent>();

            if (ai == null || health == null ||
                agent == null || !agent.enabled)
            {
                Debug.LogError(
                    selectedEnemy.enemyName +
                    " is missing a required component!"
                );

                Destroy(zombie);
                continue;
            }

            if (!agent.Warp(hit.position))
            {
                Destroy(zombie);
                continue;
            }

            ai.player = player;

            Debug.Log(
                selectedEnemy.enemyName +
                " spawned at Player Level " +
                xpManager.currentLevel
            );

            return true;
        }

        return false;
    }

    private EnemyType SelectEnemy()
    {
        List<EnemyType> availableEnemies =
            new List<EnemyType>();

        float totalWeight = 0f;

        foreach (EnemyType enemy in enemyTypes)
        {
            if (enemy.enemyPrefab == null)
                continue;

            if (xpManager.currentLevel >= enemy.unlockLevel)
            {
                availableEnemies.Add(enemy);
                totalWeight += enemy.spawnWeight;
            }
        }

        if (availableEnemies.Count == 0)
            return null;

        float randomWeight = Random.Range(0f, totalWeight);

        foreach (EnemyType enemy in availableEnemies)
        {
            randomWeight -= enemy.spawnWeight;

            if (randomWeight <= 0f)
                return enemy;
        }

        return availableEnemies[
            availableEnemies.Count - 1
        ];
    }
}
