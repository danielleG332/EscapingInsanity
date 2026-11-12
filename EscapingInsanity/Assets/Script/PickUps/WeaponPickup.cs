using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private string weaponID;
    [SerializeField] private int weaponDamage = 1;

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

        if (melee != null)
        {
            melee.PickUpWeapon(weaponDamage);

            Debug.Log(
                weaponID + " picked up! Damage: " + weaponDamage
            );

            Destroy(gameObject);
        }
    }
}