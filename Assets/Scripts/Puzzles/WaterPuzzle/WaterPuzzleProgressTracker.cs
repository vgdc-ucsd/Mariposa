using UnityEngine;

public class WaterPuzzleProgressTracker : MonoBehaviour
{
    public static WaterPuzzleProgressTracker Instance;
    public bool Puzzle1Complete { get; private set; }
    public bool Puzzle2Complete { get; private set; }
    public bool Puzzle3Complete { get; private set; }
    public bool Puzzle3Unlocked { get; private set; } = false;
    [SerializeField] private ItemData pipeItemSO;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool CanStartPuzzle(int puzzleNumber)
    {
        switch (puzzleNumber)
        {
            case 1:
                return true;

            case 2:
                if (!Puzzle1Complete)
                {
                    DialogueManager.Instance.PlayDialogue("pipe_two_no_one");
                    return false;
                }
                return true;

            case 3:
                if (!Puzzle2Complete)
                {
                    DialogueManager.Instance.PlayDialogue("pipe_three_no_others");
                    return false;
                }

                if (!Puzzle3Unlocked)
                {
                    if (!InventoryManager.Instance.GetInventory().TryConsumeItem(pipeItemSO))
                    {
                        DialogueManager.Instance.PlayDialogue("pipe_missing");
                        return false;
                    }
                    Puzzle3Unlocked = true;
                }
                return true;

            default:
                Debug.LogWarning($"Unknown puzzle number {puzzleNumber}");
                return false;
        }
    }

    public void MarkPuzzle1Complete() => Puzzle1Complete = true;
    public void MarkPuzzle2Complete() => Puzzle2Complete = true;
    public void MarkPuzzle3Complete() => Puzzle3Complete = true;
}
