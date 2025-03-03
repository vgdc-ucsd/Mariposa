using UnityEngine;

public class BlockPuzzleInteractable : GenericInteractable
{
    [SerializeField] private BlockPuzzle puzzle;

    private void Start()
    {
        // if (!puzzle.Initialized) puzzle.Initialize();
    }

    public override void OnInteract(IControllable controllable)
    {
        PuzzlePopupManager.Instance.ActivePuzzle = puzzle.gameObject;
        base.OnInteract(controllable);
    }


    private void Update()
    {
        if (isActiveAndEnabled == puzzle.isActiveAndEnabled) gameObject.SetActive(!puzzle.isActiveAndEnabled);
    }
}
