using Unity.VisualScripting;
using UnityEngine;

/*
Add this script as a component to an item to make the item's effect and visibility in game disable upon contact with player.
Enables the item to be "picked up"
Currently the item is not added to any inventory but solely sets the object's active state to false
*/
public class ItemPickup : MonoBehaviour
{

    [Header("Pickup Settings")]
    [SerializeField] protected InventoryItemSO item;
    [SerializeField] protected InventoryType targetInventory = InventoryType.Mariposa;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    
    protected virtual void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (item != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = item.lowResSprite;
        }
    }
    
    /// <summary>
    /// Called when this item is picked up.
    /// Adds the item to the specified inventory and then destroys this pickup.
    /// </summary>
    public virtual void OnPickup(GameObject collector)
    {
        if (InventoryManager.Instance != null && item != null)
        {
            InventoryManager.Instance.AddItem(targetInventory, item);
        }
        else
        {
            Debug.LogError("InventoryManager.Instance or item is null!");
        }
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPickup(other.gameObject);
        }
    }
}
