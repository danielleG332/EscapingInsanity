
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

        if (spawner != null)
            spawner.ZombieDefeated();

        StartCoroutine(RemoveAfterDeathAnimation());
    }

    IEnumerator RemoveAfterDeathAnimation()
    {
        yield return new WaitForSeconds(deathAnimationDuration);
        Destroy(gameObject);
    }
}
