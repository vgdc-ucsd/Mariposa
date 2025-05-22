using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class KeycardPickup : ItemPickup
{
    public GameObject TurnstileCollision;
    public Sprite UnlockedSprite;
    public SpriteRenderer TurnstileSR;


    public override void OnInteract(IControllable controllable)
    {
        base.OnInteract(controllable);
        TurnstileCollision.SetActive(false);
        TurnstileSR.sprite = UnlockedSprite;
    }
}
