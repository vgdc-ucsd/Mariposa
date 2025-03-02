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
    
    [Header("Active Character Inventory")]
    [SerializeField] private InventoryType activeCharacterInventory = InventoryType.Mariposa;

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
            {
                Debug.Log("invevntory close");
                CloseInventory();
            }
            else
            {
                Debug.Log("invevntory open");
                OpenInventory();
            }
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
        var toolInventoryDict = InventoryManager.Instance.GetAllItems(activeCharacterInventory);
        var mementoInventoryDict = InventoryManager.Instance.GetAllItems(InventoryType.Mementos);

        int toolSlotIndex = 0;
        foreach (var kvp in toolInventoryDict)
        {
            if (toolSlotIndex < toolsSlots.Length)
            {
                toolsSlots[toolSlotIndex].SetSlot(kvp.Key, kvp.Value);
                toolSlotIndex++;
            }
        }

        // Populate memento slots
        int mementoSlotIndex = 0;
        foreach (var kvp in mementoInventoryDict)
        {
            if (mementoSlotIndex < mementosSlots.Length)
            {
                mementosSlots[mementoSlotIndex].SetSlot(kvp.Key, kvp.Value);
                mementoSlotIndex++;
            }
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