using UnityEngine;

[CreateAssetMenu(fileName = "BatteryItem", menuName = "Inventory/BatteryItem")]
public class BatteryItem : ItemData
{
    public static BatteryItem Instance { get; private set; }

    private void OnEnable()
    {
        Instance = this;
        Name = "Battery";
        ID = 1;
        Type = InventoryItemType.SINGLE_USE;
    }
}