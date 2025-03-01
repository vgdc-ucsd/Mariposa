using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUIManager : MonoBehaviour
{
    [Header("Panels & Buttons")]
    [SerializeField] private GameObject inventoryPanel; 
    [SerializeField] private UnityEngine.UI.Button closeButton;
    [SerializeField] private UnityEngine.UI.Button toolsTabButton;
    [SerializeField] private UnityEngine.UI.Button mementosTabButton;

    [Header("Panels for Each Tab")]
    [SerializeField] private GameObject toolsPanel;     
    [SerializeField] private GameObject mementosPanel;  

    [Header("Item Details UI")]
    [SerializeField] private Image centerItemIcon;
    [SerializeField] private TextMeshProUGUI centerItemDescription;

    [Header("Slots")]
    [SerializeField] private InventoryUISlot[] toolsSlots;   
    [SerializeField] private InventoryUISlot[] mementosSlots;

    private bool isOpen = false;

    private void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseInventory);

        if (toolsTabButton != null)
            toolsTabButton.onClick.AddListener(() => ShowTab(true));
        
        if (mementosTabButton != null)
            mementosTabButton.onClick.AddListener(() => ShowTab(false));
        foreach (var slot in toolsSlots)
        {
            slot.OnSlotClicked = OnSlotClicked;
        }
        foreach (var slot in mementosSlots)
        {
            slot.OnSlotClicked = OnSlotClicked;
        }
        
        inventoryPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryPanel.activeSelf)
                CloseInventory();
            else
                OpenInventory();
        }
    }
    
    private void OnSlotClicked(InventoryItemSO item)
    {
        if (centerItemIcon != null)
        {
            centerItemIcon.sprite = item.sprite;
            centerItemIcon.enabled = true;
        }
        if (centerItemDescription != null)
        {
            centerItemDescription.text = $"{item.Name}\n\n{item.FlavorText}";
        }
    }
    
    public void PopulateInventory()
    {
        foreach (var slot in toolsSlots)
            slot.SetSlot(null, 0);
        foreach (var slot in mementosSlots)
            slot.SetSlot(null, 0);
        
        var inventoryDict = InventoryManager.Instance.GetAllItems(); 
        List<KeyValuePair<InventoryItemSO, int>> toolItems = new List<KeyValuePair<InventoryItemSO, int>>();
        List<KeyValuePair<InventoryItemSO, int>> mementoItems = new List<KeyValuePair<InventoryItemSO, int>>();

        foreach (var kvp in inventoryDict)
        {
            var item = kvp.Key;
            var count = kvp.Value;
            switch (item.Type)
            {
                case InventoryItemType.Single_Use:
                    toolItems.Add(kvp);
                    break;
                case InventoryItemType.Permanent:
                    toolItems.Add(kvp);
                    break;
                case InventoryItemType.Memento:
                    mementoItems.Add(kvp);
                    break;
            }
        }
        
        for (int i = 0; i < toolItems.Count && i < toolsSlots.Length; i++)
        {
            var kvp = toolItems[i];
            toolsSlots[i].SetSlot(kvp.Key, kvp.Value);
        }
        
        for (int i = 0; i < mementoItems.Count && i < mementosSlots.Length; i++)
        {
            var kvp = mementoItems[i];
            mementosSlots[i].SetSlot(kvp.Key, kvp.Value);
        }
    }
    
    public void OpenInventory()
    {
        isOpen = true;
        inventoryPanel.SetActive(true);
        PopulateInventory();
        ShowTab(true); 
    }

    private void CloseInventory()
    {
        isOpen = false;
        inventoryPanel.SetActive(false);
    }

    private void ShowTab(bool showTools)
    {
        if (toolsPanel != null) 
            toolsPanel.SetActive(showTools);
        if (mementosPanel != null) 
            mementosPanel.SetActive(!showTools);
    }
    
}
