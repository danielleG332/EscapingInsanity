
using UnityEngine;

public class PillPickup : MonoBehaviour
{
    [Header("Pill Settings")]
    public float sanityRestoreAmount = 30f;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
            return;

        PlayerSanity playerSanity =
            other.GetComponentInParent<PlayerSanity>();

        if (playerSanity != null)
        {
            collected = true;

            playerSanity.RestoreSanity(sanityRestoreAmount);

            Destroy(gameObject);
        }
    }
}