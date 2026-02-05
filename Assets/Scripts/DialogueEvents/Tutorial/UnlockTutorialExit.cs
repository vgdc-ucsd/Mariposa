using FMODUnity;
using UnityEngine;

public class UnlockTutorialExit : DialogueEvent
{
    [SerializeField] private GameObject jammedDoorInteraction;
    [SerializeField] private GameObject openDoorInteraction;

    public override void Trigger()
    {
        jammedDoorInteraction.SetActive(false);
        openDoorInteraction.SetActive(true);
        RuntimeManager.PlayOneShot(AudioEvents.SFX.exit_tutorial_unnamed);
    }
}
