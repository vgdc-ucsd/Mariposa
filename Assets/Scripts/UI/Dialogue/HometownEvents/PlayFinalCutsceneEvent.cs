using UnityEngine;

public class PlayFinalCutsceneEvent : DialogueEvent
{
    private enum AnimatorAction
    {
        PlayAnimation,
        AdvanceDialogue,
    }

    [SerializeField] private AnimatorAction action;
    [SerializeField] private EndingManager.Ending ending;

    public override void Trigger()
    {
        Debug.Log($"Triggering {Name}");
        EndingManager.Instance.CurrentEnding = ending;

        switch (action)
        {
            case AnimatorAction.PlayAnimation:
                EndingManager.Instance.PlayCutscene();
                break;
            case AnimatorAction.AdvanceDialogue:
                EndingManager.Instance.EndIdleLoop();
                break;
        }
    }
}
