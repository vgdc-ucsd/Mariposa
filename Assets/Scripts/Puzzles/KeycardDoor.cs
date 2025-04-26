using FMODUnity;
using UnityEngine;

public class KeycardDoor : Door
{
    [SerializeField] private InventoryItemSO keycard;
    [SerializeField] private InventoryType inventoryType;



    public bool CheckForKeycard()
    {
        if (keycard == null) return false;
        if (!InventoryManager.Instance.GetAllItems(inventoryType).ContainsKey(keycard)) return false;
        return (InventoryManager.Instance.GetAllItems(inventoryType)[keycard] > 0);

    }

    public void UseKeycard()
    {
        ToggleLock();
        ChangeState();
        InventoryManager.Instance.DeleteItem(inventoryType, keycard);
        RuntimeManager.PlayOneShot("event:/sfx/item/keycard/tap");
    }
}
