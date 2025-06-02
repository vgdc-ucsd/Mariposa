using UnityEngine;

public class ShowRadioEvent : DialogueEvent
{
    public override void Trigger()
    {
        StartCoroutine(TutorialManager.Instance.ShowRadio());
    }
}