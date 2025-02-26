using UnityEngine;
using UnityEngine.UI;
using TMPro;

// dialogue view. gets data from DialogueManager, but controls updating and displaying the state
public class DialogueViewManager : MonoBehaviour
{
	public GameObject DialogueWindow;
	[SerializeField] TMP_Text TextTarget;
	[SerializeField] 
	private bool isActive;
	private DialogueManager model;
	void Start () 
	{
		// toggle window visibility
		DialogueWindow.SetActive(false);
		isActive = false;

		// set model here to the singleton instance.
		model = DialogueManager.Instance;

		// set text to DialogueManager's current line
		Dialogue current = model.GetDialogue();
		TextTarget.text = current.Line;
	}
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.R))
		{
			isActive = !isActive;
			DialogueWindow.SetActive(isActive);
			Debug.Log("Toggled window");
		}
		if(isActive && Input.GetKeyDown(KeyCode.N))
		{
			model.AdvanceDialogue();

			// set TextTarget here to the next dialogue's sentence
			Dialogue current = model.GetDialogue();
			TextTarget.text = current.Line;

			Debug.Log("Cycled through message");
		}
	}
}
