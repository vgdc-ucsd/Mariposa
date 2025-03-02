using UnityEngine;
using UnityEngine.Events;

public class StillNPC : MonoBehaviour
{ 

	[SerializeField] private float squareDistance;
	[SerializeField] private Dialogue dialogueNPC;
	private DialogueManager manager;
	private bool hasSpoken = false;
	private int count;

	private void Start()
	{
		manager = DialogueManager.Instance;
		count = 0;
	}
	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.E) && (Player.ActivePlayer.transform.position - this.transform.position).sqrMagnitude < squareDistance)
		{
			Debug.Log("There was an interaction here!");
			if(hasSpoken)
				manager.AdvanceDialogue();
			else
			{
				manager.PlayDialogue(dialogueNPC);
				hasSpoken = true;
			}
			count++;

		}
	}
}
