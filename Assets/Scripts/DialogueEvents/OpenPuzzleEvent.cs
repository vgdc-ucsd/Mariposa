using UnityEngine;

public class OpenPuzzleEvent : DialogueEvent
{
    [SerializeField] private PuzzleInteractable puzzleInteractable;

    public override void Trigger()
    {
        puzzleInteractable.OnInteract(PlayerController.Instance.CurrentControllable);
    }
}
