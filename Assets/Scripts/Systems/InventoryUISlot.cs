using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUISlot : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;       
    [SerializeField] private UnityEngine.UI.Button slotButton;    
    [SerializeField] private TextMeshProUGUI countText; 

    private InventoryItemSO currentItem;            
    private int currentCount;                       
    
    public System.Action<InventoryItemSO> OnSlotClicked;
    
    public void SetSlot(InventoryItemSO item, int count)
    {
        currentItem = item;
        currentCount = count;

        if (item != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = item.lowResSprite;
            if (countText != null)
                countText.text = count > 1 ? count.ToString() : "";
        }
        else
        {
            iconImage.enabled = false;
            if (countText != null)
                countText.text = "";
        }   
    }

    private void Awake()
    {
        if (slotButton != null)
            slotButton.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        if (currentItem != null && OnSlotClicked != null)
        {
            OnSlotClicked.Invoke(currentItem);
        }
    }
}