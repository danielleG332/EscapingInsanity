using UnityEngine;
using UnityEngine.AI;

public class BossSpawner : MonoBehaviour
{
    [Header("Boss Settings")]
    public GameObject bossPrefab;
    public int unlockLevel = 10;

    [Header("Spawn Settings")]
    public Transform player;
    public float spawnRadius = 12f;
    public float minimumDistance = 6f;
    public float navMeshSearchDistance = 3f;
    public int spawnAttempts = 20;

    private XPManager xpManager;
    private bool hasSpawned = false;

    void Start()
    {
        if (player == null)
        {
            GameObject target =
                GameObject.FindGameObjectWithTag("Player");

            if (target != null)
                player = target.transform;
        }

        if (player != null)
        {
            xpManager = player.GetComponent<XPManager>();

            if (xpManager == null)
                xpManager = player.GetComponentInChildren<XPManager>();
        }

        if (bossPrefab == null)
            Debug.LogError("BossSpawner: Boss prefab is missing!");

        if (xpManager == null)
            Debug.LogError("BossSpawner: XPManager was not found!");
    }

    void Update()
    {
        if (hasSpawned || xpManager == null || player == null)
            return;

        if (xpManager.currentLevel >= unlockLevel)
        {
            SpawnBoss();
        }
    }

    void SpawnBoss()
    {
        if (hasSpawned || bossPrefab == null)
            return;

        for (int i = 0; i < spawnAttempts; i++)
        {
            Vector2 direction =
                Random.insideUnitCircle.normalized;

            float distance =
                Random.Range(minimumDistance, spawnRadius);

            Vector3 position = player.position +
                new Vector3(
                    direction.x * distance,
                    0f,
                    direction.y * distance
                );

            if (NavMesh.SamplePosition(
                position,
                out NavMeshHit hit,
                navMeshSearchDistance,
                NavMesh.AllAreas))
            {
                GameObject boss = Instantiate(
                    bossPrefab,
                    hit.position,
                    Quaternion.identity
                );

                ZombieAI ai =
                    boss.GetComponentInChildren<ZombieAI>();

                if (ai != null)
                    ai.player = player;

                hasSpawned = true;

                Debug.Log("First boss has spawned!");

                return;
            }
        }

        Debug.LogWarning(
            "BossSpawner: Could not find a valid spawn position."
        );
    }
}