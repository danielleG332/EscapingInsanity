
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public float deathAnimationDuration = 1.5f;

    private int currentHealth;
    private bool isDead = false;

    private Animator animator;
    private NavMeshAgent agent;
    private ZombieSpawner spawner;

    [Header("XP Drop")]
    public GameObject xpOrbPrefab;
    public int xpDropAmount = 25;

    public bool IsDead => isDead;

    void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    public void SetSpawner(ZombieSpawner owner)
    {
        spawner = owner;
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

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (animator != null)
            animator.SetTrigger("Die");

        // Stop the dead zombie from receiving more hits.
        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = false;

        StartCoroutine(RemoveAfterDeathAnimation());
    }

    IEnumerator RemoveAfterDeathAnimation()
    {
        yield return new WaitForSeconds(deathAnimationDuration);

        if (spawner != null)
            spawner.ZombieDefeated();

        Destroy(gameObject);
    }
}
