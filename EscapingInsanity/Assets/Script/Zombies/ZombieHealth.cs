
using UnityEngine;
using UnityEngine.AI;

public class ZombieHealth : MonoBehaviour
{
    public int maxHealth = 3;

    private int currentHealth;
    private bool isDead = false;
    private NavMeshAgent agent;

    [Header("XP Drop")]
    public GameObject xpOrbPrefab;
    public int xpDropAmount = 25;

    public bool IsDead => isDead;

    [Header("Health Kit Drop")]
    public GameObject healthKitPrefab;
    [Range(0f, 1f)]
    public float healthKitDropChance = 0f;
    [Header("Boss Key Drop")]
    public bool isBoss = false;
    public GameObject keyPrefab;
    

    void Awake()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (isDead)
            return;

        isDead = true;

        // Drop XP before destroying the zombie.
        if (xpOrbPrefab != null)
        {
            GameObject orb = Instantiate(
                xpOrbPrefab,
                transform.position + Vector3.up * 0.5f,
                Quaternion.identity
            );

            XPOrb xpOrb = orb.GetComponent<XPOrb>();

            if (xpOrb != null)
                xpOrb.xpAmount = xpDropAmount;
        }
        // Chance to drop a health kit.
        if (healthKitPrefab != null &&
            Random.value <= healthKitDropChance)
        {
            Instantiate(
                healthKitPrefab,
                transform.position + Vector3.up * 0.5f,
                Quaternion.identity
            );
        }
        if (isBoss && keyPrefab != null)
        {
            Instantiate(
                keyPrefab,
                transform.position + Vector3.up * 0.5f,
                Quaternion.identity
            );
        }

        Destroy(gameObject);
    }
}
