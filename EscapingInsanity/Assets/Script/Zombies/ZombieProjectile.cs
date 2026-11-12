using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ZombieProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float lifetime = 5f;

    private Transform target;
    private int damage;
    private float speed;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(
        Transform targetTransform,
        int projectileDamage,
        float projectileSpeed)
    {
        target = targetTransform;
        damage = projectileDamage;
        speed = projectileSpeed;

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Calculate the direction toward the target once.
        Vector3 direction =
            (target.position + Vector3.up * 0.8f - transform.position)
            .normalized;

        // Point the bullet in its travel direction.
        transform.rotation = Quaternion.LookRotation(direction);

        // Give the bullet actual forward velocity.
        rb.linearVelocity = direction * speed;

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        ZombieHealth zombie =
            other.GetComponentInParent<ZombieHealth>();

        if (zombie != null && !zombie.IsDead)
        {
            zombie.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}