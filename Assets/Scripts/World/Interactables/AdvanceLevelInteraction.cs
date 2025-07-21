using UnityEngine;

public class AdvanceLevelInteraction : Interactable
{
    public override void OnInteract(IControllable controllable)
    {
        LevelManager.Instance.LoadNextLevel();
    }
}
