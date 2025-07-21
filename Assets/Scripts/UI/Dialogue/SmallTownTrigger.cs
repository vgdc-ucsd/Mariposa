using UnityEngine;

public class SmallTownTrigger : DialogueEvent
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Trigger()
    {
        DialogueManager.Instance.PlayDialogue("small_town");
    }
}
