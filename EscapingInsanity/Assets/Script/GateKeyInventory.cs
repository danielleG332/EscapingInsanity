
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GateKeyInventory : MonoBehaviour
{
    private HashSet<string> keys = new HashSet<string>();

    [Header("Inventory UI")]
    public Image inventoryImage;

    [Header("Inventory Sprites")]
    public Sprite emptyInventory;
    public Sprite oneKeyInventory;
    public Sprite twoKeyInventory;
    public Sprite threeKeyInventory;

    void Start()
    {
        UpdateInventoryUI();
    }

    public void AddKey(string keyId)
    {
        if (keys.Add(keyId))
        {
            UpdateInventoryUI();
        }
    }

    public bool HasKey(string keyId)
    {
        return keys.Contains(keyId);
    }

    public void UseKey(string keyId)
    {
        if (keys.Remove(keyId))
        {
            UpdateInventoryUI();
        }
    }

    private void UpdateInventoryUI()
    {
        if (inventoryImage == null)
            return;

        int keyCount = keys.Count;

        if (keyCount == 0)
        {
            inventoryImage.sprite = emptyInventory;
        }
        else if (keyCount == 1)
        {
            inventoryImage.sprite = oneKeyInventory;
        }
        else if (keyCount == 2)
        {
            inventoryImage.sprite = twoKeyInventory;
        }
        else
        {
            inventoryImage.sprite = threeKeyInventory;
        }
    }
}
