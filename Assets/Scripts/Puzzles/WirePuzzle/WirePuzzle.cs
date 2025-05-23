using UnityEngine;

public class WirePuzzle : Puzzle
{
    public static WirePuzzle Instance;
    public bool IsComplete;

    private WirePuzzleDraggable[] wireDraggables;
    private WirePuzzleTail[] wireTails;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Debug.LogWarning("Tried to create more than one instance of the WirePuzzle singleton!");
            Destroy(this);
        }
        
        InitializeWires();
    }

    private void InitializeWires()
    {
        wireDraggables = GetComponentsInChildren<WirePuzzleDraggable>();
        wireTails = GetComponentsInChildren<WirePuzzleTail>();

        if (wireDraggables.Length != wireTails.Length) Debug.LogError("Invalid number of wire objects");
        else
        {
            for (int i = 0; i < wireDraggables.Length; ++i)
            {
                wireDraggables[i].InitializeWireDraggable(i);
                wireTails[i].InitializeWireTail();
            }
        }
    }

    public void OnMoveWire()
    {
        if (CheckSolution())
        {
            IsComplete = true;
            PlayerController.Instance.IsSquidUnlocked = true;
            SquidMovement.Instance.gameObject.SetActive(true);
            OnComplete();
        }
    }

    private bool CheckSolution()
    {
        foreach (WirePuzzleDraggable wpd in wireDraggables)
        {
            if (!wpd.IsMatched) return false;
        }
        return true;
    }
}
