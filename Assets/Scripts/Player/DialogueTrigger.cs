using UnityEngine;

public class DialogueTrigger : Trigger
{
    public Dialogue DialogueEvent;

    public override bool OnEnter(Body body)
    {
        if (!base.OnEnter(body)) return false;
        DialogueManager.Instance.PlayDialogue(DialogueEvent);
        Destroy(this.gameObject);
        return true;
    }
}
