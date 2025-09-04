using FMODUnity;
using UnityEngine;

public class KeycardDoor : Door
{
    [SerializeField] private ItemData keycard;
    [SerializeField] private TutorialGuardNPC guard;
    [SerializeField] private GameObject interactionTrigger;
    private bool initialInteract = true;
    private const string INITIAL_INTERACT = "turnstile_keycard";
    private const string NO_KEYCARD_DIALOGUE = "interact_turnstile_no_fix";
    private const string WITH_KEYCARD_DIALOGUE = "fix_turnstile";

    public override void Open()
    {
        base.Open();
        RuntimeManager.PlayOneShot(AudioEvents.SFX.keycard_tap);
    }

    public override void OnInteract(IControllable controllable)
    {
        if (initialInteract)
        {
            DialogueManager.Instance.PlayDialogue(INITIAL_INTERACT);
            InventoryManager.Instance.GetInventory().TryConsumeItem(keycard);
            initialInteract = false;
            return;
        }

        if (InventoryManager.Instance.GetInventory().TryConsumeItem(keycard))
        {
            DialogueManager.Instance.PlayDialogue(WITH_KEYCARD_DIALOGUE);
            guard.SetTurnstileFixed();
            interactionTrigger.SetActive(false);
            Open();
        }
        else
        {
            DialogueManager.Instance.PlayDialogue(NO_KEYCARD_DIALOGUE);
        }
    }
}
