using UnityEngine;

public class KeycardDoor : Door
{
    [SerializeField] private InventoryItemSO keycard;
    [SerializeField] private InventoryType inventoryType;

    private new void Awake()
    {
        

    }


    public bool CheckForKeycard()
    {
        if (keycard == null) return false;
        return (InventoryManager.Instance.GetAllItems(inventoryType)[keycard] > 0);
        
    }

    public void UseKeycard()
    {
        ToggleLock();
        ChangeState();
        InventoryManager.Instance.DeleteItem(inventoryType, keycard);
        Debug.Log("used keycard");
    }
}
