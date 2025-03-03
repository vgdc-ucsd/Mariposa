using UnityEngine;

public class SublevelTransitionTrigger : Trigger
{
    protected override bool OnlyOnce => true;
    protected override bool MustBePlayer => true;
    public override bool OnEnter(Body body)
    {
        if (!base.OnEnter(body)) return false;
        LevelManager.Instance.GoToNextSublevel();
        return true;
    }

    public override void OnExit(Body body)
    {
        base.OnExit(body);
    }
}