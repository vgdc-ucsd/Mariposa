using UnityEngine;

public class EnableBacktrackTrigger : Trigger
{
    protected override bool MustBePlayer => true;
    protected override bool OnlyOnce => true;
    public override bool OnEnter(Body body)
    {
        if (!base.OnEnter(body)) return false;
        //TutorialManager.Instance.backtrackTrigger.SetActive(true);
        return true;
    }
}
