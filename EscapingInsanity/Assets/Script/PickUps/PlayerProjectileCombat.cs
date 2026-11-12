
using UnityEngine;

public class PlayerProjectileCombat : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;

    public int projectileDamage = 2;
    public float projectileSpeed = 12f;
    public float fireRate = 1f;
    public float attackRange = 12f;

    [Header("Targeting")]
    public Transform aimDirection;

    [Range(0f, 180f)]
    public float targetingAngle = 60f;

    public LayerMask zombieLayers = ~0;

    private bool hasProjectileWeapon = false;
    private float nextFireTime;

    [Header("Animation")]
    public string projectileAnimationTrigger = "PistolAttack";
    public Animator animator;

    void Awake()
    {
        if (aimDirection == null)
            aimDirection = transform;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!hasProjectileWeapon || projectilePrefab == null)
            return;

        if (Time.time < nextFireTime)
            return;

        ZombieHealth target = FindNearestZombie();

        if (target != null)
        {
            Shoot(target);
            nextFireTime = Time.time + Mathf.Max(0.05f, fireRate);
        }
    }

    public void PickUpProjectileWeapon(
        GameObject prefab,
        int damage,
        float speed,
        float rate,
        float range)
    {
        if (prefab == null)
        {
            Debug.LogError("Projectile weapon has no projectile prefab!");
            return;
        }

        projectilePrefab = prefab;
        projectileDamage = damage;
        projectileSpeed = speed;
        fireRate = rate;
        attackRange = range;

        hasProjectileWeapon = true;
        nextFireTime = 0f;

        Debug.Log("Projectile weapon picked up! Damage: " + projectileDamage);
    }

    ZombieHealth FindNearestZombie()
    {
        if (aimDirection == null)
            return null;

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            attackRange,
            zombieLayers
        );

        ZombieHealth nearest = null;
        float nearestDistance = float.MaxValue;

        Vector3 forward = aimDirection.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude == 0f)
            return null;

        forward.Normalize();

        foreach (Collider hit in hits)
        {
            ZombieHealth zombie =
                hit.GetComponentInParent<ZombieHealth>();

            if (zombie == null || zombie.IsDead)
                continue;

            Vector3 direction =
                zombie.transform.position - aimDirection.position;

            direction.y = 0f;

            if (direction.sqrMagnitude == 0f)
                continue;

            float angle = Vector3.Angle(
                forward,
                direction.normalized
            );

            if (angle > targetingAngle)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                zombie.transform.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = zombie;
            }
        }

        return nearest;
    }

    void Shoot(ZombieHealth target)
    {
        if (target == null || target.IsDead)
            return;

        // Play projectile firing animation.
        if (animator != null)
            animator.SetTrigger(projectileAnimationTrigger);

        Vector3 spawnPosition = projectileSpawnPoint != null
            ? projectileSpawnPoint.position
            : transform.position + aimDirection.forward * 0.8f
                + Vector3.up * 0.8f;

        GameObject projectile = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        // Find the projectile movement script.
        ZombieProjectile projectileScript =
            projectile.GetComponent<ZombieProjectile>();

        if (projectileScript == null)
        {
            Debug.LogError(
                "Bullet prefab is missing ZombieProjectile.cs!"
            );

            Destroy(projectile);
            return;
        }

        // Initialize projectile movement and damage.
        projectileScript.Initialize(
            target.transform,
            projectileDamage,
            projectileSpeed
        );

        Debug.Log("Projectile fired at: " + target.name);
    }
}
