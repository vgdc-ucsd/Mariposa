using UnityEngine;

public class OpenBillboardEvent : DialogueEvent
{
    [SerializeField] private Puzzle billboardPuzzleObject;
    public override void Trigger()
    {
        if (!billboardPuzzleObject.IsComplete) PuzzlePopupManager.Instance.ActivePuzzle = billboardPuzzleObject.gameObject;
        // billboardPuzzleObject.gameObject.SetActive(true);
    }
}
