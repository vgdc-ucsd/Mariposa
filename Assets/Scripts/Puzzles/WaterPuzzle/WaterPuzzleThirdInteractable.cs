using UnityEngine;

public class WaterPuzzleThirdInteractable : PuzzleInteractable
{
    [SerializeField] private ItemData pipeItemSO;
    private bool isUnlocked = false;

    public override void OnInteract(IControllable controllable)
    {
        if (InventoryManager.Instance.GetInventory().TryConsumeItem(pipeItemSO) || isUnlocked)
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
