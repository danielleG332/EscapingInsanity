using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    // A unique identifier for each weapon type (e.g., "Cannon", "Sword", "Pistol")
    [SerializeField] private string weaponID;

    public string GetWeaponID()
    {
        return weaponID;
    }
}
