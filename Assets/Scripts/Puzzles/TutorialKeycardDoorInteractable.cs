using System.Collections.Generic;
using UnityEngine;

public class TutorialKeycardDoorInteractable : Interactable
{
    [SerializeField] private KeycardDoor door;
	[SerializeField] private bool hasAttemptedFix;
	[SerializeField] public GameObject KeycardLocation;
	[SerializeField] public ItemData keyData;

	// Fixing this door has NPC-like dialogue
    [SerializeField] private List<string> orderedDialogueNames;
	[SerializeField] private string lastDialogue;
    private Queue<string> dialogueQueue;

    [SerializeField] private TutorialGuardNPC guard;

	protected void Start()
	{
        if (orderedDialogueNames.Count == 0) Debug.LogWarning("NPC has no dialogue set!");
        dialogueQueue = new Queue<string>(orderedDialogueNames);
	}

    public override void OnInteract(IControllable controllable)
    {
		bool checkIfKeycard = door.CheckForKeycard();
		if (!hasAttemptedFix && door.CheckForKeycard())
		{
			Debug.Log("has NOT attempted fix. Has keycard.\n");
			KeycardLocation.SetActive(true);
			DialogueManager.Instance.PlayDialogue(GetDialogue(), false);
			hasAttemptedFix = true;
		}
		else if (hasAttemptedFix && door.CheckForKeycard())
        {
			Debug.Log("has attempted fix. Has keycard.\n");
			InventoryManager.Instance.GetInventory().TryConsumeItem(keyData);
            door.Open();
			DialogueManager.Instance.PlayDialogue(lastDialogue);
			guard.ReplaceDialogue();
        }
		else if (hasAttemptedFix && !door.CheckForKeycard()) 
		{
			Debug.Log("has attempted fix. Has no keycard.\n");
			DialogueManager.Instance.PlayDialogue(GetDialogue(), false);
		}
		else {
			Debug.Log("has NOT attempted fix. Has no keycard.\n");
		}
    }

    private string GetDialogue()
    {
        if (dialogueQueue.Count > 1) return dialogueQueue.Dequeue();
        else return dialogueQueue.Peek();
    }
}
