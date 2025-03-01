using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour, IDataPersistence
{
    /// <summary>
    /// list that acts as the inventory of the player
    /// depending on the design of the game, list can be a static-size array
    /// </summary>
    List<InventoryItemSO> Inventory = new List<InventoryItemSO>();

	/// <summary>
	/// Adds item to inventory
	/// </summary>
	/// <param name="item">Item class</param>
    public void AddItem(InventoryItemSO item)
    {
        Inventory.Add(item);
    }

	/// <summary>
	/// Deletes item from inventory
	/// </summary>
	/// <param name="item">Item class (reference)</param>
    public void DeleteItem(ref InventoryItemSO item)
    {
        Inventory.Remove(item);
    }

	/// <summary>
	/// Deletes item from inventory by name
	/// </summary>
	/// <param name="name">Item name</param>
    public void DeleteItemByName(string name)
    {
        foreach(InventoryItemSO item in Inventory)
        {
            if (item.Name == name)
            {
                Inventory.Remove(item);
                return;
            }
        }
    }

	/// <summary>
	/// Deletes item from inventory by ID
	/// </summary>
	/// <param name="id">Item ID</param>
    public void DeleteItemByID(uint id)
    {
        foreach(InventoryItemSO item in Inventory)
        {
            if (item.ID == id)
            {
                Inventory.Remove(item);
                return;
            }
        }
    }

    /** 
     * Saving and loading from game data 
     */

    /// <summary>
	/// Loads the last saved game data from the GameData object
	/// </summary>
	/// <param name="data">GameData object</param>
    public void LoadData(GameData data)
    {
        this.Inventory = data.Inventory;
    }

    /// <summary>
	/// Writes the current inventory to the GameData object
	/// </summary>
	/// <param name="data">GameData object</param>
    public void SaveData(ref GameData data)
    {
        data.Inventory = this.Inventory; 
    }
}
