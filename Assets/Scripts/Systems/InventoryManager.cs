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
    /// <summary>
    /// REWRITE THIS PART
    /// </summary>
    private Dictionary<InventoryType, Dictionary<InventoryItemSO, int>> inventories = new Dictionary<InventoryType, Dictionary<InventoryItemSO, int>>();
    [SerializeField] private TextMeshProUGUI batteryCounterText;

    public override void Awake()
    {
        base.Awake();
        // Initialize separate inventories for each type
        inventories[InventoryType.Mariposa] = new Dictionary<InventoryItemSO, int>();
        inventories[InventoryType.Unnamed] = new Dictionary<InventoryItemSO, int>();
    }

    /// <summary>
    /// Adds item to inventory
    /// </summary>
    /// <param name="type"></param>
    /// <param name="item">Item class</param>
    public void AddItem(InventoryType type, InventoryItemSO item)
    {
        var inv = inventories[type];
        if (!inv.TryAdd(item, 1))
        {
            inv[item]++;
        }
    }

    /// <summary>
    /// Deletes item from inventory
    /// </summary>
    /// <param name="type"></param>
    /// <param name="item">Item class (reference)</param>
    public void DeleteItem(InventoryType type, InventoryItemSO item)
    {
        var inv = inventories[type];
        if (inv.ContainsKey(item) && inv[item] > 0)
        {
            inv[item]--;
            if (inv[item] <= 0)
                inv.Remove(item);
        }
    }

    /// <summary>
    /// Deletes item from inventory by name
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name">Item name</param>
    public void DeleteItemByName(InventoryType type, string name)
    {
        var inv = inventories[type];
        foreach (InventoryItemSO item in new List<InventoryItemSO>(inv.Keys))
        {
            if (item.Name == name)
            {
                inv.Remove(item);
                return;
            }
        }
    }

    /// <summary>
    /// Returns all items in the specified inventory.
    /// </summary>
    public int GetItemCount(InventoryType type, InventoryItemSO item)
    {
        var inv = inventories[type];
        return inv.GetValueOrDefault(item, 0);
    }

    /// <summary>
    /// Deletes item from inventory by ID
    /// </summary>
    /// <param name="type"></param>
    /// <param name="id">Item ID</param>
    public void DeleteItemByID(InventoryType type, uint id)
    {
        var inv = inventories[type];
        foreach (InventoryItemSO item in new List<InventoryItemSO>(inv.Keys))
        {
            if (item.ID == id)
            {
                inv.Remove(item);
                return;
            }
        }
    }
    
    /*
    private void UpdateBatteryUI()
    {
        if (batteryCounterText != null)
        {
            int count = GetItemCount(InventoryType.Mariposa, BatteryItem.Instance);
            batteryCounterText.text = $"Batteries: {count}";
        }
    }
    */
    
    public Dictionary<InventoryItemSO, int> GetAllItems(InventoryType type)
    {
        return inventories[type];
    }
}