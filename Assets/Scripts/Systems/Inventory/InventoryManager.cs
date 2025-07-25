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

    void Start()
    {
        // TODO load inventory data
        mariposaInventory = new Inventory();
        unnamedInventory = new Inventory();

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

}
