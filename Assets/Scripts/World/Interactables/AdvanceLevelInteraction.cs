using FMODUnity;
using UnityEngine;

public class AdvanceLevelInteraction : Interactable
{
    public override void OnInteract(IControllable controllable)
    {
        RuntimeManager.PlayOneShot(AudioEvents.SFX.walking_up_stairs);
        LevelManager.Instance.LoadNextLevel();
    }
}
