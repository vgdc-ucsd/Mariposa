using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Item Slots")]
    [SerializeField] private InventoryUISlot[] specialItems;
    [SerializeField] private InventoryUISlot[] basicItems;

    [Header("Item Details UI")]
    [SerializeField] private Image centerItemIcon;
    [SerializeField] private TextMeshProUGUI centerItemDescription;

    void Start()
    {
        foreach (InventoryUISlot slot in specialItems)
        {
            slot.SetUI(this);
        }

        foreach (InventoryUISlot slot in basicItems)
        {
            slot.SetUI(this);
        }
    }

    public void PopulateInventory(Inventory inventory)
    {
        List<KeyValuePair<ItemData, int>> specialItemInfo = new List<KeyValuePair<ItemData, int>>();
        List<KeyValuePair<ItemData, int>> basicItemInfo = new List<KeyValuePair<ItemData, int>>();

        foreach (KeyValuePair<ItemData, int> itemInfo in inventory.GetItems())
        {
            if (itemInfo.Key.Type == InventoryItemType.SPECIAL) specialItemInfo.Add(itemInfo);
            else basicItemInfo.Add(itemInfo);
        }

        PopulateSlots(specialItems, specialItemInfo);
        PopulateSlots(basicItems, basicItemInfo);
    }

    public void OpenInventory(Inventory inventory)
    {
        gameObject.SetActive(true);
        PopulateInventory(inventory);
        centerItemDescription.text = "";
        centerItemIcon.gameObject.SetActive(false);
    }

    public void CloseInventory()
    {
        gameObject.SetActive(false);
    }

    public void CloseButton()
    {
        GameManager.Instance.HandleInventory();
    }

    public void Display(ItemData item)
    {
        centerItemDescription.text = item.FlavorText;
        centerItemIcon.sprite = item.highResSprite;
        centerItemIcon.gameObject.SetActive(true);
    }

    private void PopulateSlots(InventoryUISlot[] slots, List<KeyValuePair<ItemData, int>> itemInfoList)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            InventoryUISlot slot = slots[i];
            if (i < itemInfoList.Count)
            {
                KeyValuePair<ItemData, int> itemInfo = itemInfoList[i];
                slot.Set(itemInfo.Key, itemInfo.Value);
            }
            else
            {
                slot.Clear();
            }
        }
    }
}