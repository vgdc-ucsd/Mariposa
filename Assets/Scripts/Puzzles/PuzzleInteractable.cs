using UnityEngine;

public class PuzzleInteractable : Interactable
{
    public GameObject puzzleObject;

    private Puzzle puzzle;

    public override void OnInteract(IControllable controllable)
    {
        Debug.Log("Puzzle Interacted with!");
        if (puzzle == null) puzzle = puzzleObject.GetComponent<Puzzle>();
        if (puzzleObject == null) Debug.LogWarning($"{gameObject.name} Puzzle not assigned in inspector!");
        if (!puzzle.IsComplete) PuzzlePopupManager.Instance.ActivePuzzle = puzzleObject;
    }
}
