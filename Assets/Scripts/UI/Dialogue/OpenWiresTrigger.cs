using UnityEngine;

public class OpenWiresTrigger : DialogueEvent
{
    [SerializeField] private Puzzle wiresPuzzle;

    public override void Trigger()
    {
        if (!wiresPuzzle.IsComplete)
        {
            // wiresPuzzle.gameObject.SetActive(true);
            PuzzlePopupManager.Instance.ActivePuzzle = wiresPuzzle.gameObject;
        }
    }
}
