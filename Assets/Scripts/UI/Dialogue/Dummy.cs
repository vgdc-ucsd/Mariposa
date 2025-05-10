using UnityEngine;


class Dummy : MonoBehaviour {

	/*public DialogueManager manager;*/
	private Dialogue myDialogue;
	void Start() 
	{
		/*DialogueParser.SelfTest("ScriptFormatInline.yaml");*/
		DialogueParser parser = new DialogueParser("Assets/Art/Sprites/", "");
		this.myDialogue = parser.ParseDialogue("ScriptFormatInline.yaml");
		Debug.Log(myDialogue.Conversation.Count);
		if(DialogueManager.Instance == null)
			Debug.Log("DialogueManager issue?\n");

	}
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Z))
		{
			if(DialogueManager.Instance.IsPlayingDialogue)
			{
				Debug.Log("WE BALL\n");
				DialogueManager.Instance.TryAdvanceDialogue();
			} 
			else 
			{
				Debug.Log("BEGIN BALLING\n");
				DialogueManager.Instance.PlayDialogue(this.myDialogue);
			}
		}
	}
}
