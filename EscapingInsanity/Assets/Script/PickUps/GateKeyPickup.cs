using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateKeyPickup : MonoBehaviour
{
    public enum KeyType
    {
        Section1Key,
        Section2Key,
        Section3Key
    }

    public KeyType keyType = KeyType.Section1Key;

    private void OnTriggerEnter(Collider other)
    {
        GateKeyInventory inventory =
            other.GetComponent<GateKeyInventory>();

        if (inventory == null)
        {
            inventory = other.GetComponentInParent<GateKeyInventory>();
        }

        if (inventory != null)
        {
            string keyId = keyType.ToString();

            inventory.AddKey(keyId);

            Debug.Log("Collected: " + keyId);

            Destroy(gameObject);
        }
    }
}
