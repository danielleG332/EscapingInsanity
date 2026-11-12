using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public enum WeaponType
    {
        Melee,
        Projectile
    }

    [Header("Weapon Settings")]
    [SerializeField] private string weaponID;
    [SerializeField] private WeaponType weaponType;

    [Header("Damage")]
    [SerializeField] private int weaponDamage = 1;

    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 12f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float attackRange = 12f;

    public string GetWeaponID()
    {
        return weaponID;
    }

    public int GetWeaponDamage()
    {
        return weaponDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMeleeCombat melee =
            other.GetComponentInParent<PlayerMeleeCombat>();

        PlayerProjectileCombat ranged =
            other.GetComponentInParent<PlayerProjectileCombat>();

        if (weaponType == WeaponType.Melee && melee != null)
        {
            melee.PickUpWeapon(weaponDamage);

            Debug.Log(weaponID + " picked up!");

            Destroy(gameObject);
        }
        else if (weaponType == WeaponType.Projectile && ranged != null)
        {
            if (projectilePrefab == null)
            {
                Debug.LogWarning(
                    "Projectile prefab is missing from " + weaponID
                );
                return;
            }

            ranged.PickUpProjectileWeapon(
                projectilePrefab,
                weaponDamage,
                projectileSpeed,
                fireRate,
                attackRange
            );

            Debug.Log(weaponID + " picked up!");

            Destroy(gameObject);
        }
    }
}