
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GateInteraction : MonoBehaviour
{
    public string requiredKeyId = "Section1Key";
    public SlidingGate gate;

    private bool playerNearby = false;
    private bool isUnlocked = false;

    private void Update()
    {
        if (!playerNearby || isUnlocked)
            return;

        // New Input System: detect the E key
        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            GateKeyInventory inventory =
                FindFirstObjectByType<GateKeyInventory>();

            if (inventory != null &&
                inventory.HasKey(requiredKeyId))
            {
                inventory.UseKey(requiredKeyId);

                isUnlocked = true;
                gate.OpenGate();

                Debug.Log("Gate unlocked!");
            }
            else
            {
                Debug.Log("This gate is locked. Find its key!");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            Debug.Log("Press E to open the gate.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}
