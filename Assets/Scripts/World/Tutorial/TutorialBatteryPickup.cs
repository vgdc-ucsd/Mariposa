using UnityEngine;

public class TutorialBatteryPickup : ItemPickup
{
    [SerializeField] private string dialogueName;
    [SerializeField] AnimatorSFX animatorSFX;

    public override void OnInteract(IControllable controllable)
    {
        base.OnInteract(controllable);
        animatorSFX.Stop();
        DialogueManager.Instance.PlayDialogue(dialogueName);
    }
}
