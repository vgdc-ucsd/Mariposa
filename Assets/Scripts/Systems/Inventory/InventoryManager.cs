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
    [SerializeField] private InventoryUIManager ui;
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
        if (Player.ActivePlayer.Character.Id == CharID.Mariposa)
        {

        }
        //ui.OpenInventory()
    }

    public Inventory GetInventory()
    {
        if (Player.ActivePlayer.Character.Id == CharID.Mariposa) return mariposaInventory;
        else return unnamedInventory;
    }
}
