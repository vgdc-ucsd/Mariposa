using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple NPC that plays a sequence of dialogue in order. It repeats the last line of dialogue forever.
/// </summary>
public class BasicNPC : NPC
{
    [SerializeField] private List<string> orderedDialogueNames;
    private Queue<string> dialogueQueue;

    protected override void Start()
    {
        base.Start();
        if (orderedDialogueNames.Count == 0) Debug.LogWarning("NPC has no dialogue set!");
        dialogueQueue = new Queue<string>(orderedDialogueNames);
    }

    protected override string GetDialogue()
    {
        if (dialogueQueue.Count > 1) return dialogueQueue.Dequeue();
        else return dialogueQueue.Peek();
    }
}
