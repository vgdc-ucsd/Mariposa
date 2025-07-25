using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Overlays;

public enum InventoryType
{
    Mariposa,
    Unnamed,
}

public class InventoryManager : Singleton<InventoryManager>
{
    [SerializeField] private InventoryUI mariposaUI;
    [SerializeField] private InventoryUI unnamedUI;
    private Inventory mariposaInventory;
    private Inventory unnamedInventory;

    [Header("Preset Items")]
    [Tooltip("Mariposa: Beebo, Radio")]
    [SerializeField] private List<ItemData> mariposaItems;
    [Tooltip("Unnnamed: Grapple Hook, Radio")]
    [SerializeField] private List<ItemData> unnamedItems;

    void Start()
    {
        mariposaInventory = new Inventory();
        unnamedInventory = new Inventory();
        LoadInventory(true); // Should run starting in Tutorial

        GameManager.Instance.RegisterStartAction(GameState.INVENTORY, EnterInventory);
        GameManager.Instance.RegisterExitAction(GameState.INVENTORY, ExitInventory);

        ExitInventory();
    }

    private void EnterInventory()
    {
        Time.timeScale = 0.0f;
        if (Player.ActivePlayer.Data.characterID == CharID.Mariposa)
        {
            mariposaUI.OpenInventory(mariposaInventory);
        }
        else
        {
            unnamedUI.OpenInventory(unnamedInventory);
        }
    }

    private void ExitInventory()
    {
        mariposaUI.CloseInventory();
        unnamedUI.CloseInventory();
    }

    public Inventory GetInventory()
    {
        if (Player.ActivePlayer.Data.characterID == CharID.Mariposa) return mariposaInventory;
        else return unnamedInventory;
    }

    /*
     Preset Inventories
     tutorial:
        mariposa: beebo, radio
        unnamed: grappling hook
     every other stage:
        mariposa: beebo, radio
        unnamed: grappling hook, radio
    */
    public void LoadInventory(bool isTutorial)
    {
        foreach(ItemData item in mariposaItems) { mariposaInventory.AddItem(item); }
        foreach (ItemData item in unnamedItems) { if (!isTutorial || item.Name != "Radio") { unnamedInventory.AddItem(item); } }
    }
}
