using UnityEngine;

public class PlayDialogueEvent : DialogueEvent
{
    [SerializeField] private string dialogueToPlay;

    public override void Trigger()
    {
        DialogueManager.Instance.PlayDialogue(dialogueToPlay);
    }
}
