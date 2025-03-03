using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/*
Add this script as a component to an item to make the item's effect and visibility in game disable upon contact with player.
Enables the item to be "picked up"
Currently the item is not added to any inventory but solely sets the object's active state to false
*/
public class ItemPickup : Interactable
{

    [Header("Pickup Settings")]
    [SerializeField] protected InventoryItemSO item;
    [SerializeField] protected SpriteRenderer spriteRenderer;

    public override bool DestroyOnInteract => true;

    protected override void Awake()
    {
        base.Awake();
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
    public override void OnInteract(IControllable controllable)
    {
        Debug.Log("picked up");
        if (InventoryManager.Instance != null && item != null)
        {
            InventoryType type;
            if (controllable is BeeMovement) type = InventoryType.Mariposa;
            else if (controllable is PlayerMovement pm)
            {
                if (pm.Parent.Character.Id == CharID.Mariposa)
                {
                    type = InventoryType.Mariposa;
                }
                else
                {
                    type = InventoryType.Unnamed;
                }
            }
            else
            {
                Debug.LogError("No suitable Inventory to add item to");
                return;
            }
            InventoryManager.Instance.AddItem(type, item);
        }
        else
        {
            Debug.LogError("InventoryManager.Instance or item is null!");
        }
        //Destroy(gameObject);
    }
}
