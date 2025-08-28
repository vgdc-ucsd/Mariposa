using UnityEngine;

public class SetMariposaPierTriggerEvent : DialogueEvent
{
    [SerializeField] private DialogueTrigger mariposaPierStartTrigger;

    public override void Start()
    {
        base.Start();

        mariposaPierStartTrigger.gameObject.SetActive(false);
    }

    public override void Trigger()
    {
        mariposaPierStartTrigger.gameObject.SetActive(true);
    }
}
