using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    // A unique identifier for each weapon type (e.g., "Cannon", "Sword", "Pistol")
    [SerializeField] private string weaponID;

    public string GetWeaponID()
    {
        return weaponID;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMeleeCombat melee =
            other.GetComponentInParent<PlayerMeleeCombat>();

        if (melee != null && weaponID == "Bat")
        {
            melee.PickUpWeapon();

            Destroy(gameObject);
        }
        if (melee != null && weaponID == "Axe")
        {
            melee.PickUpWeapon();

            Destroy(gameObject);
        }
        if (melee != null && weaponID == "Hockey Stick")
        {
            melee.PickUpWeapon();

            Destroy(gameObject);
        }
        if (melee != null && weaponID == "Knife")
        {
            melee.PickUpWeapon();

            Destroy(gameObject);
        }
    }
}
