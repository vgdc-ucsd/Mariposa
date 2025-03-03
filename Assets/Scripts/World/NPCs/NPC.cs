using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class NPC : Interactable
{

	[SerializeField] private float squareDistance;
	[SerializeField] private Dialogue dialogueNPC;
	private DialogueManager manager;
	private bool isSpeaking = false;
	private int count;
	[SerializeField] private InRangeDetector interactionRange;

	protected override void Start()
	{
		manager = DialogueManager.Instance;
		count = 0;
	}

    public override void OnInteract(IControllable controllable)
    {
		if(count >= dialogueNPC.Conversation.Count)
		{
			manager.DialogueWindow.SetActive(false);
			count = 0;
			isSpeaking = false;
			PlayerController.Instance.ToggleMovementLock();
		}
		else if (count == 0)
		{
			manager.PlayDialogue(dialogueNPC);
			isSpeaking = true;
			count++;
			PlayerController.Instance.ToggleMovementLock();
		}
		else
		{
			manager.TryAdvanceDialogue();
			count++;
		}
    }
}
