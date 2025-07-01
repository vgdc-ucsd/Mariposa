using UnityEngine;

public class WaterPuzzleThirdInteractable : PuzzleInteractable
{
    [SerializeField] private InventoryItemSO pipeItemSO;
    private bool isUnlocked = false;

    public override void OnInteract(IControllable controllable)
    {
        if (InventoryManager.Instance.TryConsumeItem(InventoryType.Mariposa, pipeItemSO.ID) || isUnlocked)
        {
            isUnlocked = true;
            base.OnInteract(controllable);
        }
        else
        {
            DialogueManager.Instance.PlayDialogue("pipe_missing");
        }
    }
}
