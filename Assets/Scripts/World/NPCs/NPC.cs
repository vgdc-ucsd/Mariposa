using System.Collections;
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
	private bool canInteract = true;

	protected override void Start()
	{
		manager = DialogueManager.Instance;
		count = 0;
		usedDialogue = dialogueNPC;
	}

	// necessary to wait one frame to set canInteract, so that player doesn't close dialogue and re-open dialogue on the same frame
	private IEnumerator SetDialogueFinished()
	{
        usedDialogue = finishDialogue;
		yield return new WaitForEndOfFrame();
		canInteract = true;
    }


    public override void OnInteract(IControllable controllable)
    {
        if (!manager.IsPlayingDialogue && canInteract)
		{
            manager.PlayDialogue(usedDialogue, () => StartCoroutine(SetDialogueFinished()));
			canInteract = false;
		}
    }
}
