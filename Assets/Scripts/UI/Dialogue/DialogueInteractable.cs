using UnityEngine;

public class DialogueInteractable : Interactable
{
    [SerializeField] private string dialogue;
    [SerializeField] private bool destroyOnInteract;

    public override void OnInteract(IControllable controllable)
    {
        DialogueManager.Instance.PlayDialogue(dialogue);
        if (destroyOnInteract) Destroy(GetComponentInChildren<InteractionTrigger>());
    }
}
