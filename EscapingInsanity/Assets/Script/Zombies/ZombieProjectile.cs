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

        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction =
            (target.position + Vector3.up * 0.8f - transform.position)
            .normalized;

        rb.MovePosition(
            rb.position + direction * speed * Time.fixedDeltaTime
        );

        if (direction.sqrMagnitude > 0f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
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