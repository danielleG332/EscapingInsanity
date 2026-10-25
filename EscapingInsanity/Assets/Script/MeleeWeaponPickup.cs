
using UnityEngine;

public class MeleeWeaponPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerMeleeCombat combat =
            other.GetComponent<PlayerMeleeCombat>();

        if (combat == null)
            combat = other.GetComponentInParent<PlayerMeleeCombat>();

        if (combat != null)
        {
            combat.PickUpWeapon();
            Destroy(gameObject);
        }
    }
}
