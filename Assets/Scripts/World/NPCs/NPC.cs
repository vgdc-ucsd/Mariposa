using System.Collections.Generic;
using UnityEngine;

public abstract class NPC : Interactable
{
	protected abstract string GetDialogue();

	public override void OnInteract(IControllable controllable)
	{
		DialogueManager.Instance.PlayDialogue(GetDialogue(), false);
	}
}
