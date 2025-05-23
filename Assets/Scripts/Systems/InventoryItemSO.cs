using UnityEngine;

public enum InventoryItemType {
	Permanent,
	Single_Use,
	Memento
}


[CreateAssetMenu(fileName = "InventoryItem", menuName = "InventoryItem")]
public class InventoryItemSO : ScriptableObject
{
	public string Name;
	public string FlavorText;
	public uint ID; 
	public InventoryItemType Type;

	[Header("UI Images")]
	public Sprite lowResSprite;
	public Sprite highResSprite; 
}
