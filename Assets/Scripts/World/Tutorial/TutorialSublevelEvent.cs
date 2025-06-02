using UnityEngine;

public class TutorialSublevelEvent : DialogueEvent
{
    public override void Trigger()
    {
        LevelManager.Instance.GoToNextSublevel();
    }
}
