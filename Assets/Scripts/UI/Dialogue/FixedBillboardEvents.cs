using UnityEngine;

public class FixedBillboardEvents : MonoBehaviour
{
    public void TriggerFixedDialogue()
    {
        DialogueManager.Instance.PlayDialogue("billboard_fixed_wires_fixed");
    }

    public void TriggerSublevelSwitch()
    {
        return;
    }
}
