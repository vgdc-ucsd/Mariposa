using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class KeycardPickup : ItemPickupMVP
{
    public GameObject TurnstileCollision;
    public Sprite UnlockedSprite;
    public SpriteRenderer TurnstileSR;
    private EventInstance KeycardPickupEvent;


    protected override void Pickup()
    {
        base.Pickup();
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
