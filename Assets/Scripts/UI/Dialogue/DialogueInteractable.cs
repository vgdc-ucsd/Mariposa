using UnityEngine;

public class DialogueInteractable : Interactable
{
    [SerializeField] private string dialogue;

    public override void OnInteract(IControllable controllable)
    {
        DialogueManager.Instance.PlayDialogue(dialogue);
    }
}
