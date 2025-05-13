using UnityEngine;

public class DialogueTriggerYAML : Trigger
{
    public string DialoguePath;

    public override bool OnEnter(Body body)
    {
        if (!base.OnEnter(body)) return false;
		DialogueParser parser = new DialogueParser("Assets/Art/Sprites/", "");
		Debug.Log("made it here\n");
		Dialogue dialogueEvent = parser.ParseDialogue(DialoguePath);
        DialogueManager.Instance.PlayDialogue(dialogueEvent);
        Destroy(this.gameObject);
        return true;
    }
}
