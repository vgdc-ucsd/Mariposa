using UnityEngine;

public class PlayFinalCutsceneEvent : DialogueEvent
{
    private enum AnimatorAction
    {
        PlayAnimation,
        AdvanceCutscene,
    }

    [SerializeField] private AnimatorAction action;
    [Tooltip("Only sets EndingManager's ending value when action is set to PlayAnimation")]
    [SerializeField] private EndingManager.Ending ending;

    public override void Trigger()
    {
        switch (action)
        {
            case AnimatorAction.PlayAnimation:
                EndingManager.Instance.CurrentEnding = ending;
                EndingManager.Instance.PlayCutscene();
                break;
            case AnimatorAction.AdvanceCutscene:
                EndingManager.Instance.AdvanceCutscene();
                break;
        }
    }
}
