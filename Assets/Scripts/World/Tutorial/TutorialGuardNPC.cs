using System.Collections.Generic;
using UnityEngine;

public class TutorialGuardNPC : NPC
{
    [SerializeField] private List<string> orderedDialogueNames;
    private Queue<string> dialogueQueue;
    [SerializeField] private string extraDialogue;
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

    /// <summary>
    /// Called when the player fixes the turnstile
    /// </summary>
    public void AddNewDialogue()
    {
        dialogueQueue.Enqueue(extraDialogue);
    }
}
