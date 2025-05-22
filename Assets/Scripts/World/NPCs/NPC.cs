using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class NPC : Interactable
{

	[SerializeField] private List<string> dialogueNames;
	//[SerializeField] private Dialogue finishDialogue;
	private DialogueManager manager;
	private bool isSpeaking = false;
	private int index;

	protected override void Start()
	{
		manager = DialogueManager.Instance;
		index = 0;
	}

	public override void OnInteract(IControllable controllable)
	{
		Debug.LogError("TODO");
		/* if(index >= usedDialogue.Conversation.Count)
		{
			if(manager.finishedTypewriter)
			{
				manager.DialogueWindow.SetActive(false);
				index = 0;
				isSpeaking = false;
				this.usedDialogue = finishDialogue;
				PlayerController.Instance.ToggleMovementLock();
			}
			else
			{
				manager.TryAdvanceDialogue();
			}
		}
		else if (index == 0)
		{
			manager.PlayDialogue(usedDialogue);
			isSpeaking = true;
			index++;
			//PlayerController.Instance.ToggleMovementLock();
		}
		else
		{
			//if(manager.finishedTypewriter)
			//	count++;
			//manager.TryAdvanceDialogue();
		} */
	}
}
