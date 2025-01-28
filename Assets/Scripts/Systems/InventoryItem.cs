using UnityEngine;

public enum ItemType {
	Permanent,
	Single_Use,
	Memento
}

public class InventoryItem 
{
	public const string Name;
	public const string FlavorText;
	public const uint ID; 
	public const enum ItemType Type;

	/// <summary>
	/// Creates a record representing an item. 
	/// </summary>
	/// <param name="name">Item name</param>
	/// <param name="text">Item flavor text</param>
	/// <param name="id">Item ID, for later use</param>
	/// <param name="type">Which category of item does this item fit into?</param>
	public InventoryItem(string name, string text, uint id, enum ItyemType type)
	{
		this.Name = name;
		this.FlavorText = text;
		this.ID = id;
		this.Type = type;
	}
}
