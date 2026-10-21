
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GateKeyPickup : MonoBehaviour
{
    public string keyId = "Section1Key";

    private void OnTriggerEnter(Collider other)
    {
        GateKeyInventory inventory =
            other.GetComponent<GateKeyInventory>();

        if (inventory == null)
            inventory = other.GetComponentInParent<GateKeyInventory>();

        if (inventory != null)
        {
            inventory.AddKey(keyId);
            Debug.Log("Collected key: " + keyId);
            Destroy(gameObject);
        }
    }
}
