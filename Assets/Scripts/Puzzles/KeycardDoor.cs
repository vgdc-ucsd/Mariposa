using FMODUnity;
using UnityEngine;

public class KeycardDoor : Door
{
    [SerializeField] private ItemData keycard;
    [SerializeField] private InventoryType inventoryType;

    [SerializeField] private TutorialGuardNPC guard;
    public Sprite UnlockedSprite;
    public SpriteRenderer TurnstileSR;
    public GameObject TurnstileCollision;

    public bool CheckForKeycard()
    {
        return InventoryManager.Instance.GetInventory().HasItem(keycard);
    }

    public void Open()
    {
        ToggleLock();
        ChangeState();
        RuntimeManager.PlayOneShot("event:/sfx/item/keycard/tap");
        TurnstileCollision.SetActive(false);
        // TurnstileSR.sprite = UnlockedSprite;
    }
}
