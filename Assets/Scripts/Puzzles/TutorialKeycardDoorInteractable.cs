using System.Collections.Generic;
using UnityEngine;

public class TutorialKeycardDoorInteractable : Interactable
{
    [SerializeField] private KeycardDoor door;
	[SerializeField] private bool hasAttemptedFix;
	[SerializeField] public GameObject KeycardLocation;

	// Fixing this door has NPC-like dialogue
    [SerializeField] private List<string> orderedDialogueNames;
	[SerializeField] private string lastDialogue;
    private Queue<string> dialogueQueue;

	protected void Start()
	{
        if (orderedDialogueNames.Count == 0) Debug.LogWarning("NPC has no dialogue set!");
        dialogueQueue = new Queue<string>(orderedDialogueNames);
	}

    public override void OnInteract(IControllable controllable)
    {
		bool checkIfKeycard = door.CheckForKeycard();
		if (!hasAttemptedFix)
		{
			KeycardLocation.SetActive(true);
			DialogueManager.Instance.PlayDialogue(GetDialogue(), false);
			hasAttemptedFix = true;
		}
		else if (door.CheckForKeycard())
        {
			DialogueManager.Instance.PlayDialogue(lastDialogue);
            door.UseKeycard();
        }
		else {
			DialogueManager.Instance.PlayDialogue(GetDialogue(), false);
		}
    }

    private string GetDialogue()
    {
        if (dialogueQueue.Count > 1) return dialogueQueue.Dequeue();
        else return dialogueQueue.Peek();
    }
}
