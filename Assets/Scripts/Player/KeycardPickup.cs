using UnityEngine;

public class KeycardPickup : ItemPickupMVP
{
    public GameObject TurnstileCollision;
    public Sprite UnlockedSprite;
    public SpriteRenderer TurnstileSR;


    protected override void Pickup()
    {
        base.Pickup();
        TurnstileCollision.SetActive(false);
        TurnstileSR.sprite = UnlockedSprite;
    }
}
