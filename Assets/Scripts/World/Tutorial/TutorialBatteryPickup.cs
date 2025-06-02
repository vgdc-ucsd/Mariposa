using UnityEngine;

public class TutorialBatteryPickup : ItemPickup
{
    [SerializeField] private string dialogueName;

    public override void OnInteract(IControllable controllable)
    {
        base.OnInteract(controllable);
        DialogueManager.Instance.PlayDialogue(dialogueName);
    }
}
