using UnityEngine;

public class ItemPickupMVP : MonoBehaviour
{
    public InventoryItemSO Item;
    public InventoryType Type;


    protected virtual void Pickup()
    {
        InventoryManager.Instance.AddItem(Type, Item);
        Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if(collision.CompareTag("Bee") || collision.CompareTag("Player")) Pickup();
    }
}
