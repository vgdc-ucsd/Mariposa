using UnityEngine;

public class ShowRadioEvent : DialogueEvent
{
    public override void Trigger()
    {
        TutorialManager.Instance.ShowRadio();
    }
}