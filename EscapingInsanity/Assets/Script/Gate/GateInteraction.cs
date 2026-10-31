using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GateInteraction : MonoBehaviour
{
    public enum GateKeyType
    {
        Section1Key,
        Section2Key,
        Section3Key
    }

    [Header("Gate Settings")]
    public GateKeyType requiredKey = GateKeyType.Section1Key;
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

            string requiredKeyId = requiredKey.ToString();

            if (inventory != null &&
                inventory.HasKey(requiredKeyId))
            {
                inventory.UseKey(requiredKeyId);

                isUnlocked = true;

                if (gate != null)
                {
                    gate.OpenGate();
                    Debug.Log(requiredKeyId + " unlocked the gate!");
                }
                else
                {
                    Debug.LogError("No SlidingGate assigned!");
                }
            }
            else
            {
                Debug.Log("Gate locked! You need " + requiredKeyId);
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
