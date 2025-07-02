using FMODUnity;
using UnityEngine;

public class KeycardDoor : Door
{
    [SerializeField] private ItemData keycard;
    [SerializeField] private InventoryType inventoryType;

    public bool CheckForKeycard()
    {
        return InventoryManager.Instance.GetInventory().HasItem(keycard);
    }

    public void UseKeycard()
    {
        ToggleLock();
        ChangeState();
        InventoryManager.Instance.GetInventory().TryConsumeItem(keycard);
        RuntimeManager.PlayOneShot("event:/sfx/item/keycard/tap");
    }
}
