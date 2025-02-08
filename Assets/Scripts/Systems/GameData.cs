using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// JSON serializable class that is used to store the game data. 
/// </summary>
[System.Serializable]
public class GameData
{
    // REMOVE THIS VARIABLE LATER! This is just to test the saving :)
    public int TEST_keyStrokeCount;

    // Player inventory
    // Not sure if there should be two fields? aren't there two different inventories
    public List<InventoryItem> Inventory;
    /// <summary>
    /// Constructor for the GameData object
    /// Creates a new object with the default values
    /// </summary>
    public GameData()
    {
        Inventory = new List<InventoryItem>();
        this.TEST_keyStrokeCount = 0;
    }
}