using UnityEngine;
using UnityEngine.Events;

public class DialogueOnlyNPC : MonoBehaviour
{

	[SerializeField] private float squareDistance;
	[SerializeField] private Dialogue dialogueNPC;
	private DialogueManager manager;
	private bool isSpeaking = false;
	private int count;
	[SerializeField] private InRangeDetector interactionRange;

	private void Start()
	{
		manager = DialogueManager.Instance;

		// TODO: add method to DialogueManager that says when dialogue is over.
		count = 0;
		/*interactionRange.SetTarget(Player.ActivePlayer);*/
	}
	private void Update()
	{
		// TODO: more editor-friendly interaction distance calculation
		if(Input.GetKeyDown(KeyCode.E) && (Player.ActivePlayer.transform.position - this.transform.position).sqrMagnitude < squareDistance)
		/*if(Input.GetKeyDown(KeyCode.E) && interactionRange.IsTargetInRange())*/
		{
			Debug.Log("There was an interaction here!");
			if(count >= dialogueNPC.Conversation.Count)
			{
				// TODO: method for after-dialogue behavior (for subclassing)
				manager.DialogueWindow.SetActive(false);
				count = 0;
				isSpeaking = false;
			}
			else if (count == 0)
			{
				manager.PlayDialogue(dialogueNPC);
				isSpeaking = true;
				count++;
			}
			else
			{
				manager.TryAdvanceDialogue();
				count++;
			}
		}
		// NOTE: this is never called if dialogue disables movement.
		else if (isSpeaking && (Player.ActivePlayer.transform.position - this.transform.position).sqrMagnitude >= squareDistance)
		{
			Debug.Log("Dismissing dialogue window (out of range)");
			manager.DialogueWindow.SetActive(false);
			count = 0;
			isSpeaking = false;
		}

		// TODO: Create method in PlayerController to freeze movement.
		if(isSpeaking)
		{
			PlayerMovement.Instance.SetMoveDir(Vector2.zero);
		}
	}
}
