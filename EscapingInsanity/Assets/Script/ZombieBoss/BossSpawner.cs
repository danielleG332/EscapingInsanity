
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [System.Serializable]
    public class BossInfo
    {
        public string bossName;
        public GameObject bossPrefab;
        public Transform spawnPoint;
        public int requiredLevel = 1;

        [HideInInspector]
        public bool hasSpawned = false;
    }

    [Header("Boss Settings")]
    public BossInfo[] bosses;

    [Header("Player")]
    public Transform player;

    public int currentPlayerLevel = 1;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }
    }

    void Update()
    {
        if (player == null)
            return;

        foreach (BossInfo boss in bosses)
        {
            if (boss.hasSpawned)
                continue;

            if (currentPlayerLevel >= boss.requiredLevel)
            {
                SpawnBoss(boss);
            }
        }
    }

    void SpawnBoss(BossInfo boss)
    {
        if (boss.bossPrefab == null || boss.spawnPoint == null)
        {
            Debug.LogWarning("Missing prefab or spawn point for " + boss.bossName);
            return;
        }

        Instantiate(
            boss.bossPrefab,
            boss.spawnPoint.position,
            boss.spawnPoint.rotation
        );

        boss.hasSpawned = true;

        Debug.Log(boss.bossName + " has spawned!");
    }

    public void SetPlayerLevel(int level)
    {
        currentPlayerLevel = level;
    }
}
