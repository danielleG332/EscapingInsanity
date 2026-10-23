
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

            if (animator != null)
            {
                animator.SetBool("IsWalking", false);

                if (Time.time >= nextAttackTime)
                {
                    animator.SetTrigger("Attack");
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
        }
    }

    public void Defeat()
    {
        Destroy(gameObject);
    }
}
