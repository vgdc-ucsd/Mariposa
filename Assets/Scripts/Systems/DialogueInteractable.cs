using UnityEngine;

public class DialogueInteractable : Interactable
{
    [SerializeField] private string dialogueToTrigger;

    public override void OnInteract(IControllable controllable)
    {
        DialogueManager.Instance.PlayDialogue(dialogueToTrigger);
    }

    protected override void Awake()
    {
        base.Awake();
    }
}
