using UnityEngine;

public class HideRadioEvent : DialogueEvent
{
    public override void Trigger()
    {
        TutorialManager.Instance.HideRadio();
    }
}