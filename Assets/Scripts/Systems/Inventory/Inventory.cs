using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private Dictionary<ItemData, int> items;

    /// <summary>
    /// Adds item to inventory
    /// </summary>
    /// <param name="item">Item class</param>
    public void AddItem(ItemData item)
    {
        if (items.ContainsKey(item)) items[item]++;
        else items.Add(item, 1);
        GameEvents.Instance.Trigger<UpdateTriggers>();
    }

    /// <summary>
    /// Checks if the item exists in the inventory
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool HasItem(ItemData item)
    {
        return items.ContainsKey(item);
    }

    /// <summary>
    /// Tries to use an item from inventory by ID
    /// </summary>
    /// <returns>True if item used successfully, false if item not found<\returns>
    public bool TryConsumeItem(ItemData item)
    {
        if (items.ContainsKey(item))
        {
            items[item]--;
            if (items[item] == 0) items.Remove(item);
            return true;
        }
        else return false;
    }
} 