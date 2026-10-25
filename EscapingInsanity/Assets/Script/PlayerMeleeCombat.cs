
using System.Collections;
using UnityEngine;

public class PlayerMeleeCombat : MonoBehaviour
{
    [Header("Melee Settings")]
    public float attackRange = 1.8f;
    public float attackRadius = 0.8f;
    public int attackDamage = 1;
    public float attackCooldown = 1f;
    public float hitDelay = 0.3f;

    [Header("References")]
    public Animator animator;
    public Transform attackPoint;
    public LayerMask zombieLayers = ~0;

    private bool hasMeleeWeapon = false;
    private bool isAttacking = false;

    void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!hasMeleeWeapon || isAttacking)
            return;

        ZombieHealth target = FindNearbyZombie();

        if (target != null)
            StartCoroutine(PerformAttack());
    }

    public void PickUpWeapon()
    {
        hasMeleeWeapon = true;
        Debug.Log("Melee weapon picked up!");
    }

    ZombieHealth FindNearbyZombie()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            attackRange,
            zombieLayers
        );

        ZombieHealth nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            ZombieHealth zombie = hit.GetComponentInParent<ZombieHealth>();

            if (zombie == null || zombie.IsDead)
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

    IEnumerator PerformAttack()
    {
        isAttacking = true;

        if (animator != null)
            animator.SetTrigger("MeleeAttack");

        // Wait for the swing to reach the target.
        yield return new WaitForSeconds(hitDelay);

        Vector3 center = attackPoint != null
            ? attackPoint.position
            : transform.position + transform.forward * attackRange * 0.65f
                + Vector3.up * 0.8f;

        Collider[] hits = Physics.OverlapSphere(
            center,
            attackRadius,
            zombieLayers
        );

        foreach (Collider hit in hits)
        {
            ZombieHealth zombie = hit.GetComponentInParent<ZombieHealth>();

            if (zombie != null && !zombie.IsDead)
                zombie.TakeDamage(attackDamage);
        }

        yield return new WaitForSeconds(
            Mathf.Max(0f, attackCooldown - hitDelay)
        );

        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 center = attackPoint != null
            ? attackPoint.position
            : transform.position + transform.forward * attackRange * 0.65f
                + Vector3.up * 0.8f;

        Gizmos.DrawWireSphere(center, attackRadius);
    }
}
