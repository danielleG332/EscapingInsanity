using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public Transform player;

    [Header("Combat")]
    public float attackRange = 1.8f;
    public float attackCooldown = 1.5f;
    public float damagePerAttack = 10f;

    [Header("Movement")]
    public float destinationUpdateRate = 0.2f;

    private NavMeshAgent agent;
    private Animator animator;
    private PlayerHealth playerHealth;

    private float nextAttackTime;
    private float nextDestinationUpdate;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

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
            playerHealth = player.GetComponent<PlayerHealth>();

            if (playerHealth == null)
                playerHealth = player.GetComponentInParent<PlayerHealth>();

            if (playerHealth == null)
                playerHealth = player.GetComponentInChildren<PlayerHealth>();
        }
    }

    void Update()
    {
        if (player == null || agent == null ||
            !agent.enabled || !agent.isOnNavMesh)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distance > attackRange)
        {
            agent.isStopped = false;

            if (Time.time >= nextDestinationUpdate)
            {
                agent.SetDestination(player.position);
                nextDestinationUpdate =
                    Time.time + destinationUpdateRate;
            }

            if (animator != null)
            {
                animator.SetBool(
                    "IsWalking",
                    agent.velocity.sqrMagnitude > 0.1f
                );
            }
        }
        else
        {
            agent.isStopped = true;

            Vector3 direction =
                player.position - transform.position;

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
}