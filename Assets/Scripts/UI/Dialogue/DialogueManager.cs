using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// dialogue model
public class DialogueManager : Singleton<DialogueManager>
{
	Queue<Dialogue> Conversation;
	void PlayDialogue(List<Dialogue> dialogue)
	{
		Conversation = new Queue<Dialogue>(dialogue);
	}

	public void AdvanceDialogue()
	{
		Dialogue prevHead = Conversation.Dequeue();
		Conversation.Enqueue(prevHead);
	}
	public Dialogue GetDialogue()
	{
		return Conversation.Peek();
	}
}
