using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class KeycardPickup : ItemPickup
{
    public GameObject TurnstileCollision;
    public Sprite UnlockedSprite;
    public SpriteRenderer TurnstileSR;
    private EventInstance KeycardPickupEvent;


    public override void OnInteract(IControllable controllable)
    {
        base.OnInteract(controllable);
        TurnstileCollision.SetActive(false);
        KeycardPickupEvent.setParameterByNameWithLabel("ItemType", "Default");
        KeycardPickupEvent.start();
        KeycardPickupEvent.release();
        TurnstileSR.sprite = UnlockedSprite;
    }

    void Start()
    {
        KeycardPickupEvent = RuntimeManager.CreateInstance("event:/sfx/item/pickup");
    }
}
