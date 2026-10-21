using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateKeyInventory : MonoBehaviour
{
    private HashSet<string> keys = new HashSet<string>();

    public void AddKey(string keyId)
    {
        keys.Add(keyId);
    }

    public bool HasKey(string keyId)
    {
        return keys.Contains(keyId);
    }

    public void UseKey(string keyId)
    {
        keys.Remove(keyId);
    }
}
