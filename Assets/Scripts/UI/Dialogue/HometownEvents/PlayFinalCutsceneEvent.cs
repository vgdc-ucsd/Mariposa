using UnityEngine;

public class PlayFinalCutsceneEvent : DialogueEvent
{
    private enum AnimatorAction
    {
        PlayAnimation,
        EndIdleLoop,
    }

    [SerializeField] private AnimatorAction action;
    [SerializeField] private bool isGoodEnding;

    public override void Trigger()
    {
        Debug.Log($"Triggering {Name}");
        EndingManager.Instance.IsGoodEnding = isGoodEnding;

        switch (action)
        {
            case AnimatorAction.PlayAnimation:
                EndingManager.Instance.PlayCutscene();
                break;
            case AnimatorAction.EndIdleLoop:
                EndingManager.Instance.EndIdleLoop();
                break;
        }
    }
}
