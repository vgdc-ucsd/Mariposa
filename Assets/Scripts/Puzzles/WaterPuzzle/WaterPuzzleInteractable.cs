using UnityEngine;

public class WaterPuzzleInteractable : PuzzleInteractable
{
    [SerializeField] private int puzzleNumber = 1;

    public override void OnInteract(IControllable controllable)
    {
        if (WaterPuzzleProgressTracker.Instance.CanStartPuzzle(puzzleNumber))
        {
            base.OnInteract(controllable);
        }
    }
}
