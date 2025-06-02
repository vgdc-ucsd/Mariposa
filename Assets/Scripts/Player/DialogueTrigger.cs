using UnityEngine;

public class DialogueTrigger : Trigger
{
    public string DialogueName;

    public override bool OnEnter(Body body)
    {
        if (!base.OnEnter(body)) return false;
        DialogueManager.Instance.PlayDialogue(DialogueName);
        Destroy(this.gameObject);
        return true;
    }
}
