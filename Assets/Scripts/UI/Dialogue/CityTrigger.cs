using UnityEngine;

public class CityTrigger : DialogueEvent
{
    public override void Trigger()
    {
        DialogueManager.Instance.PlayDialogue("city_charm");
    }
}
