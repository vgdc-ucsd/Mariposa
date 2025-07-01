using UnityEngine;

public class SwitchSublevelsEvent : DialogueEvent
{
    public override void Trigger()
    {
        LevelManager.Instance.GoToNextSublevel();
    }
}
