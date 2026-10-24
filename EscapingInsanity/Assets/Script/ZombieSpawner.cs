
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public Transform player;

    public int maxZombies = 5;
    public float spawnRadius = 12f;
    public float minimumDistance = 5f;

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
        {
            Debug.LogError(
                "Assign the zombie prefab and player!");
            return;
        }

        for (int i = 0; i < maxZombies; i++)
        {
            SpawnZombie();
        }
    }

    void SpawnZombie()
    {
        for (int attempt = 0; attempt < 20; attempt++)
        {
            Vector2 random =
                Random.insideUnitCircle.normalized *
                Random.Range(minimumDistance, spawnRadius);

            Vector3 candidate = player.position +
                new Vector3(random.x, 0f, random.y);

            if (NavMesh.SamplePosition(
                candidate, out NavMeshHit hit,
                3f, NavMesh.AllAreas))
            {
                GameObject zombie = Instantiate(
                    zombiePrefab, hit.position,
                    Quaternion.identity);

                ZombieAI ai = zombie.GetComponent<ZombieAI>();

                if (ai != null)
                {
                    ai.player = player;
                }
                else
                {
                    Debug.LogError(
                        "Zombie prefab needs ZombieAI!");
                    Destroy(zombie);
                }

                return;
            }
        }

        Debug.LogWarning(
            "Could not find a valid zombie spawn position.");
    }
}
