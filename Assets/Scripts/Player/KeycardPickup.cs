using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class KeycardPickup : ItemPickup
{
    public GameObject TurnstileCollision;
    public Sprite UnlockedSprite;
    public SpriteRenderer TurnstileSR;
    public string fixedDoorDialogueName;
    [SerializeField] private TutorialGuardNPC guard;
    public override void OnInteract(IControllable controllable)
    {
        base.OnInteract(controllable);
        TurnstileCollision.SetActive(false);
        TurnstileSR.sprite = UnlockedSprite;
        DialogueManager.Instance.PlayDialogue(fixedDoorDialogueName);
        guard.AddNewDialogue();
    }
}
