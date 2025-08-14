using Unity.VisualScripting;
using UnityEngine;

// literally the exact same as InteractionTrigger except for onexit, which forces the interact prompt to close
// very hacky. not proud of this implementation
public class SpecialSwitchInteractionTrigger : InteractionTrigger
{

    public override void OnExit(Body body)
    {
        base.OnExit(body);
        InGameUI.Instance.InteractPrompt(false);
    }

}