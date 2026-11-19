
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
    public float stuckCheckTime = 3f;
    public float minimumMovement = 0.1f;

    [Header("Audio")]
    public AudioClip groanLoop;
    private AudioSource idleSource;

    private NavMeshAgent agent;
    private Animator animator;
    private PlayerHealth playerHealth;
    private ZombieHealth zombieHealth;

    private float nextAttackTime;
    private float nextDestinationUpdate;
    private float stuckTimer;

    private Vector3 lastPosition;

    void Awake()
    {
        agent = GetComponentInParent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        zombieHealth = GetComponent<ZombieHealth>();
    }

    void Start()
    {
        FindPlayer();

        lastPosition = transform.position;

        SetupGroan();
    }

    void FindPlayer()
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
                playerHealth =
                    player.GetComponentInParent<PlayerHealth>();

            if (playerHealth == null)
                playerHealth =
                    player.GetComponentInChildren<PlayerHealth>();
        }
    }

    void Update()
    {
        // Stop all AI behavior when defeated.
        if (zombieHealth != null && zombieHealth.IsDead)
        {
            SetWalkingAnimation(false);
            return;
        }

        if (player == null)
        {
            FindPlayer();
            return;
        }

        if (agent == null || !agent.enabled ||
            !agent.isOnNavMesh)
        {
            SetWalkingAnimation(false);
            return;
        }

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distance > attackRange)
        {
            ChasePlayer();
        }
        else
        {
            AttackPlayer();
        }
    }

    void ChasePlayer()
    {
        agent.isStopped = false;

        // Update the destination periodically.
        if (Time.time >= nextDestinationUpdate)
        {
            agent.SetDestination(player.position);

            nextDestinationUpdate =
                Time.time + destinationUpdateRate;
        }

        CheckMovement();

        SetWalkingAnimation(
            !agent.pathPending &&
            agent.hasPath &&
            agent.velocity.sqrMagnitude > 0.1f
        );
    }

    void AttackPlayer()
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

        SetWalkingAnimation(false);

        if (Time.time >= nextAttackTime)
        {
            if (animator != null)
                animator.SetTrigger("Attack");

            if (playerHealth != null)
                playerHealth.TakeDamage(damagePerAttack);

            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void CheckMovement()
    {
        if (agent.pathPending || !agent.hasPath)
            return;

        float movedDistance = Vector3.Distance(
            transform.position,
            lastPosition
        );

        if (movedDistance < minimumMovement)
        {
            stuckTimer += Time.deltaTime;
        }
        else
        {
            stuckTimer = 0f;
        }

        if (stuckTimer >= stuckCheckTime)
        {
            agent.ResetPath();

            nextDestinationUpdate = 0f;

            stuckTimer = 0f;
        }

        lastPosition = transform.position;
    }

    void SetWalkingAnimation(bool walking)
    {
        if (animator != null)
            animator.SetBool("IsWalking", walking);
    }

    void SetupGroan()
    {
        if (groanLoop == null)
            return;

        if (!TryGetComponent(out idleSource))
            idleSource = gameObject.AddComponent<AudioSource>();

        if (idleSource == null)
        {
            Debug.LogWarning("Could not create AudioSource on " + name);
            return;
        }

        idleSource.playOnAwake = false;
        idleSource.loop = true;
        idleSource.spatialBlend = 1f;
        idleSource.rolloffMode = AudioRolloffMode.Linear;
        idleSource.minDistance = 2f;
        idleSource.maxDistance = 15f;
        idleSource.volume = 0.4f;
        idleSource.pitch = Random.Range(0.85f, 1.15f);
        idleSource.clip = groanLoop;
        idleSource.Play();
        idleSource.time = Random.Range(0f, groanLoop.length * 0.9f);

    }
}
