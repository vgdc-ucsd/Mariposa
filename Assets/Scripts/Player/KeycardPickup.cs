using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class KeycardPickup : ItemPickup
{
    public GameObject TurnstileCollision;
    public Sprite UnlockedSprite;
    public SpriteRenderer TurnstileSR;
    private EventInstance KeycardPickupEvent;
    public Dialogue fixedDoorDialogue;

    public override void OnInteract(IControllable controllable)
    {
        base.OnInteract(controllable);
        TurnstileCollision.SetActive(false);
        KeycardPickupEvent.setParameterByNameWithLabel("ItemType", "Default");
        KeycardPickupEvent.start();
        KeycardPickupEvent.release();
        TurnstileSR.sprite = UnlockedSprite;
        DialogueManager.Instance.PlayDialogue(fixedDoorDialogue);
    }

    void Start()
    {
        KeycardPickupEvent = RuntimeManager.CreateInstance("event:/sfx/item/pickup");
    }
}
