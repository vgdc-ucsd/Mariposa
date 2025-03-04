using UnityEngine;

public class SublevelTransitionTrigger : Trigger
{
    protected override bool OnlyOnce => true;
    protected override bool MustBePlayer => true;
    public Dialogue dialogue;

    public override bool OnEnter(Body body)
    {
        if (!base.OnEnter(body)) return false;
        if(dialogue != null) {
            DialogueManager.Instance.PlayDialogue(dialogue, () => LevelManager.Instance.GoToNextSublevel());
        }
        else LevelManager.Instance.GoToNextSublevel();
        return true;
    }

    public override void OnExit(Body body)
    {
        base.OnExit(body);
    }
}