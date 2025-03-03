using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class NPC : Interactable
{

	[SerializeField] private Dialogue dialogueNPC;
	[SerializeField] private Dialogue finishDialogue;
	private DialogueManager manager;
	private bool isSpeaking = false;
	private int count;
	private Dialogue usedDialogue;

	protected override void Start()
	{
		manager = DialogueManager.Instance;
		count = 0;
		usedDialogue = dialogueNPC;
	}

    public override void OnInteract(IControllable controllable)
    {
		if(count >= usedDialogue.Conversation.Count)
		{
			if(manager.finishedTypewriter)
			{
				manager.DialogueWindow.SetActive(false);
				count = 0;
				isSpeaking = false;
				this.usedDialogue = finishDialogue;
				PlayerController.Instance.ToggleMovementLock();
			}
			else
			{
				manager.TryAdvanceDialogue();
			}
		}
		else if (count == 0)
		{
			manager.PlayDialogue(usedDialogue);
			isSpeaking = true;
			count++;
			PlayerController.Instance.ToggleMovementLock();
		}
		else
		{
			if(manager.finishedTypewriter)
				count++;
			manager.TryAdvanceDialogue();
		}
    }
}
