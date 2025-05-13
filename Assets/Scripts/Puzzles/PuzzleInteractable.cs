using UnityEngine;

public class PuzzleInteractable : Interactable
{
    public GameObject puzzleObject;

    public override void OnInteract(IControllable controllable)
    {
        Debug.Log("Puzzle Interacted with!");
        if (puzzleObject == null) Debug.LogWarning($"{gameObject.name} Puzzle not assigned in inspector!");
        PuzzlePopupManager.Instance.ActivePuzzle = puzzleObject;
    }
}
