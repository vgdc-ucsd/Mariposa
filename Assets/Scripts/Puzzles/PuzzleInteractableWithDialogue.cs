using UnityEngine;

public class PuzzleInteractableWithDialogue : PuzzleInteractable
{
    [SerializeField] private DialogueEvent beforeInteractEvent;
    [SerializeField] private DialogueEvent AfterInteractEvent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    public override void OnInteract(IControllable controllable)
    {
        if (!PuzzleInstance.IsComplete) beforeInteractEvent.Trigger();
    }
}
