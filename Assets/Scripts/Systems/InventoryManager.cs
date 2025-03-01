using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class InventoryManager : Singleton<InventoryManager>
{
    /// <summary>
    /// REWRITE THIS PART
    /// </summary>
    private Dictionary<InventoryItemSO, int> inventory = new Dictionary<InventoryItemSO, int>();
    [SerializeField] private TextMeshProUGUI batteryCounterText;

	/// <summary>
	/// Adds item to inventory
	/// </summary>
	/// <param name="item">Item class</param>
    public void AddItem(InventoryItemSO item)
    {
        if (inventory.ContainsKey(item))
        {
            inventory[item]++;
        }
        else
        {
            inventory[item] = 1;
        }
        UpdateBatteryUI();
    }

	/// <summary>
	/// Deletes item from inventory
	/// </summary>
	/// <param name="item">Item class (reference)</param>
    public void DeleteItem(InventoryItemSO item)
    {
        if (inventory.ContainsKey(item) && inventory[item] > 0)
        {
            inventory[item]--;
            if (inventory[item] <= 0)
            {
                inventory.Remove(item);
            }
        }
        UpdateBatteryUI();
    }

	/// <summary>
	/// Deletes item from inventory by name
	/// </summary>
	/// <param name="name">Item name</param>
    public void DeleteItemByName(string name)
    {
        foreach(InventoryItemSO item in new List<InventoryItemSO>(inventory.Keys))
        {
            if (item.Name == name)
            {
                inventory.Remove(item);
                return;
            }
        }
    }

    public int GetItemCount(InventoryItemSO item)
    {
        return inventory.ContainsKey(item) ? inventory[item] : 0;
    }
    
	/// <summary>
	/// Deletes item from inventory by ID
	/// </summary>
	/// <param name="id">Item ID</param>
    public void DeleteItemByID(uint id)
    {
        foreach(InventoryItemSO item in new List<InventoryItemSO>(inventory.Keys))
        {
            if (item.ID == id)
            {
                inventory.Remove(item);
                return;
            }
        }
    }
    
    private void UpdateBatteryUI()
    {
        if (batteryCounterText != null)
        {
            int count = GetItemCount(BatteryItem.Instance);
            batteryCounterText.text = $"Batteries: {count}";
        }
    }
    
    public Dictionary<InventoryItemSO, int> GetAllItems()
    {
        return inventory;
    }

}