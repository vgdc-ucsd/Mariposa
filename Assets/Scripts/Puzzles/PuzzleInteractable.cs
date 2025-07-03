using UnityEngine;

public class PuzzleInteractable : Interactable
{
    public Puzzle PuzzleInstance;

    protected override void Start()
    {
        base.Start();
        if (PuzzleInstance == null) Debug.LogWarning($"{gameObject.name} Puzzle not assigned in inspector!");
    }

    public override void OnInteract(IControllable controllable)
    {
        if (!PuzzleInstance.IsComplete) PuzzlePopupManager.Instance.ActivePuzzle = PuzzleInstance.gameObject;
    }
}
