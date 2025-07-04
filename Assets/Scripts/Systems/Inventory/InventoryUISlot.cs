using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventoryUISlot : MonoBehaviour, IPointerEnterHandler
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;       
    [SerializeField] private UnityEngine.UI.Button slotButton;    
    [SerializeField] private TextMeshProUGUI countText;

    private ItemData item;
    private InventoryUI ui;

    public void Set(ItemData item, int count)
    {
        this.item = item;
        countText.text = count.ToString();
        iconImage.sprite = item.lowResSprite;
        iconImage.gameObject.SetActive(true);
    }

    public void Clear()
    {
        item = null;
        countText.text = "";
        iconImage.sprite = null;
        iconImage.gameObject.SetActive(false);
    }

    public void SetUI(InventoryUI ui)
    {
        this.ui = ui;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null) return;
        ui.Display(item);
    }
}