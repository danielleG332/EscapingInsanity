
using UnityEngine;

public class HealthKitPickup : MonoBehaviour
{
    [Header("Healing Settings")]
    public float healAmount = 30f;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
            return;

        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
        {
            collected = true;

            playerHealth.Heal(healAmount);

            Destroy(gameObject);
        }
    }
}
