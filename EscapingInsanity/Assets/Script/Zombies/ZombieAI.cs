
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class ZombieAI : MonoBehaviour
{
    public Transform player;
    public float attackRange = 1.8f;
    public float attackCooldown = 1.5f;

    private NavMeshAgent agent;
    private Animator animator;
    private float nextAttackTime;

    public float damagePerAttack = 10f;
    private PlayerHealth playerHealth;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        if (player == null)
        {
            GameObject target = GameObject.FindGameObjectWithTag("Player");

            if (target != null)
                player = target.transform;
        }

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (player == null || agent == null || !agent.isOnNavMesh)
            return;

        float distance = Vector3.Distance(
            transform.position, player.position
        );

        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

            if (animator != null)
                animator.SetBool("IsWalking", agent.velocity.sqrMagnitude > 0.1f);
        }

        else
        {
            agent.isStopped = true;

            // Face the player while attacking.
            Vector3 direction = player.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    Quaternion.LookRotation(direction),
                    agent.angularSpeed * Time.deltaTime
                );
            }

            if (animator != null)
                animator.SetBool("IsWalking", false);

            if (Time.time >= nextAttackTime)
            {
                if (animator != null)
                    animator.SetTrigger("Attack");

                if (playerHealth != null)
                    playerHealth.TakeDamage(damagePerAttack);

                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    public void Defeat()
    {
        ZombieLifetime lifetime = GetComponent<ZombieLifetime>();

        if (lifetime != null)
            lifetime.Defeat();
    }
}
