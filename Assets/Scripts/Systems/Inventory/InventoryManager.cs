using UnityEngine;
using System.Collections.Generic;
using TMPro;

public enum InventoryType
{
    Mariposa,
    Unnamed,
}

public class InventoryManager : Singleton<InventoryManager>
{
    private Inventory mariposaInventory;
    private Inventory unnamedInventory;

    void Start()
    {
        // TODO load inventory data
        mariposaInventory = new Inventory();
        unnamedInventory = new Inventory();
    }

    public void OpenInventory(bool open)
    {
        
    }
}
