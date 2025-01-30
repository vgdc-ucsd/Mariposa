using UnityEngine;

public enum InventoryItemType {
	Permanent,
	Single_Use,
	Memento
}

public class InventoryItem 
{
	public string Name;
	public string FlavorText;
	public uint ID; 
	public InventoryItemType Type;

	/// <summary>
	/// Creates a record representing an item. 
	/// </summary>
	/// <param name="name">Item name</param>
	/// <param name="text">Item flavor text</param>
	/// <param name="id">Item ID, for later use</param>
	/// <param name="type">Which category of item does this item fit into?</param>
	public InventoryItem(string name, string text, uint id, InventoryItemType type)
	{
		this.Name = name;
		this.FlavorText = text;
		this.ID = id;
		this.Type = type;
	}
}
