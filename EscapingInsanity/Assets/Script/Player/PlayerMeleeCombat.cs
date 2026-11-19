
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

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip[] swingSounds;
    public AudioClip hitSound;

    public float soundPitch = 0.75f;
    public float minSoundInterval = 0.6f;
    private float nextSoundTime;



    void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        
        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();

        if (sfxSource != null)
            sfxSource.pitch = soundPitch;
    }

    void Update()
    {
        if (!hasMeleeWeapon || isAttacking)
            return;

        ZombieHealth target = FindNearbyZombie();

        if (target != null)
            StartCoroutine(PerformAttack());
    }

    public void PickUpWeapon(int weaponDamage)
    {
        hasMeleeWeapon = true;
        attackDamage = weaponDamage;

        Debug.Log("Melee weapon picked up! Damage: " + attackDamage);
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

        if (sfxSource != null && swingSounds.Length > 0 && Time.time >= nextSoundTime)
        {
            sfxSource.PlayOneShot(swingSounds[Random.Range(0, swingSounds.Length)]);
            nextSoundTime = Time.time + minSoundInterval;
        }

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

            if (sfxSource != null & hitSound != null && Time.time >= nextSoundTime)
            {
                sfxSource.PlayOneShot(hitSound);
                nextSoundTime = Time.time + minSoundInterval;
            }
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
