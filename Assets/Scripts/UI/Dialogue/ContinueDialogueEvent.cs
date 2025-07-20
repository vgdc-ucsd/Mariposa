using System.Runtime.CompilerServices;
using UnityEngine;

public class ContinueDialogueEvent : DialogueEvent
{
    [SerializeField] private string dialogueToPlay;
    public override void Trigger()
    {
        DialogueManager.Instance.PlayDialogue(dialogueToPlay);
    }
}
