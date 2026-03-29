using FMODUnity;
using UnityEngine;

public class HometownDoorInteractable : Interactable
{
    [SerializeField] private string dialogueName;
    public override void OnInteract(IControllable controllable)
    {
        DialogueManager.Instance.PlayDialogue(dialogueName);
    }
}
