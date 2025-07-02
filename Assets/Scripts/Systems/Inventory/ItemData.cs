using UnityEngine;

public enum InventoryItemType
{
	SPECIAL,
	PERMANENT,
	SINGLE_USE,
	MEMENTO
}


[CreateAssetMenu(fileName = "InventoryItem", menuName = "InventoryItem")]
public class ItemData : ScriptableObject
{
	public string Name;
	public string FlavorText;
	public uint ID;
	public InventoryItemType Type;
	public ItemPickup.ItemSFXType ItemSFXType = ItemPickup.ItemSFXType.Default;

	[Header("UI Images")]
	public Sprite lowResSprite;
	public Sprite highResSprite;
}
