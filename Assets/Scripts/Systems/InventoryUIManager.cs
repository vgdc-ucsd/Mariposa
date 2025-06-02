using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUIManager : MonoBehaviour
{
    [Header("Panels & Buttons")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private UnityEngine.UI.Button closeButton;
    
    [Header("Panels for Inventory Sections")]
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
    
    [Header("UI Themes")]
    [SerializeField] private Sprite mariposaCenterImage;
    [SerializeField] private Sprite unnamedCenterImage;
    [SerializeField] private Sprite mariposaSlotBackground;
    [SerializeField] private Sprite unnamedSlotBackground;
    [SerializeField] private Sprite mariposaInventoryBackground;
    [SerializeField] private Sprite unnamedInventoryBackground;
    [SerializeField] private Sprite mariposaInfoPanel;
    [SerializeField] private Sprite unnamedInfoPanel;
    [SerializeField] private Sprite mariposaPermaSlot;
    [SerializeField] private Sprite unnamedPermaSlot;
    
    [Header("Themed UI Elements")]
    [SerializeField] private Image centerPanelImage;
    [SerializeField] private Image inventoryBackgroundImage;
    [SerializeField] private Image infoPanelImage;

    private bool isOpen = false;

    private void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseInventory);

        foreach (var slot in toolsSlots)
            slot.OnSlotClicked = OnSlotClicked;
        foreach (var slot in mementosSlots)
            slot.OnSlotClicked = OnSlotClicked;
        inventoryPanel.SetActive(false);
        if (centerItemIcon != null)
            centerItemIcon.enabled = false;
        if (centerItemDescription != null)
            centerItemDescription.text = "";    
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
            centerItemIcon.sprite = item.highResSprite;
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
        var activeInv = InventoryManager.Instance.GetAllItems(activeCharacterInventory);
        List<KeyValuePair<InventoryItemSO, int>> leftItems = new List<KeyValuePair<InventoryItemSO, int>>();
        List<KeyValuePair<InventoryItemSO, int>> rightItems = new List<KeyValuePair<InventoryItemSO, int>>();
        foreach (var kvp in activeInv)
        {
            if (kvp.Key.Type == InventoryItemType.Single_Use)
                leftItems.Add(kvp);
            else if (kvp.Key.Type == InventoryItemType.Permanent || kvp.Key.Type == InventoryItemType.Memento)
                rightItems.Add(kvp);
        }
        int leftSlotIndex = 0;
        foreach (var kvp in leftItems)
        {
            if (leftSlotIndex < toolsSlots.Length)
            {
                toolsSlots[leftSlotIndex].SetSlot(kvp.Key, kvp.Value);
                leftSlotIndex++;
            }
        }
        int rightSlotIndex = 0;
        foreach (var kvp in rightItems)
        {
            if (rightSlotIndex < mementosSlots.Length)
            {
                mementosSlots[rightSlotIndex].SetSlot(kvp.Key, kvp.Value);
                rightSlotIndex++;
            }
        }
    }

    public void OpenInventory()
    {
        isOpen = true;
        inventoryPanel.SetActive(true);
        if (toolsPanel != null)
            toolsPanel.SetActive(true);
        if (mementosPanel != null)
            mementosPanel.SetActive(true);
        UpdateUITheme();
        PopulateInventory();
    }

    private void CloseInventory()
    {
        isOpen = false;
        inventoryPanel.SetActive(false);
    }
    
    public void SetActiveCharacterInventory(InventoryType newActive)
    {
        activeCharacterInventory = newActive;
        PopulateInventory();
    }

    private void UpdateUITheme()
    {
        Sprite centerSprite = null;
        Sprite bgSprite = null;
        Sprite infoSprite = null;
        Sprite slotBg = null;
        Sprite permaSlot = null;

        switch (activeCharacterInventory)
        {
            case InventoryType.Mariposa:
                bgSprite = mariposaInventoryBackground;
                infoSprite = mariposaInfoPanel;
                slotBg = mariposaSlotBackground;
                permaSlot = mariposaPermaSlot;
                break;

            case InventoryType.Unnamed:
                bgSprite = unnamedInventoryBackground;
                infoSprite = unnamedInfoPanel;
                slotBg = unnamedSlotBackground;
                permaSlot = unnamedPermaSlot;
                break;

            default:
                Debug.LogWarning($"No UI theme defined for inventory type: {activeCharacterInventory}");
                break;
        }

        foreach (var slot in toolsSlots)
            slot.SetPermaSlotGraphic(permaSlot);

        foreach (var slot in mementosSlots)
        {
            slot.SetSlotBackground(slotBg);
        }

        if (inventoryBackgroundImage != null)
            inventoryBackgroundImage.sprite = bgSprite;

        if (infoPanelImage != null)
            infoPanelImage.sprite = infoSprite;
    }
}