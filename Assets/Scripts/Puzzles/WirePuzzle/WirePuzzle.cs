using UnityEngine;

public class WirePuzzle : Puzzle
{
    public static WirePuzzle Instance;

    private WirePuzzleDraggable[] wireDraggables;
    private WirePuzzleTail[] wireTails;
    private WirePuzzleNode[] wireNodes;

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
        wireNodes = GetComponentsInChildren<WirePuzzleNode>();

        if (wireDraggables.Length != wireTails.Length) Debug.LogError("Invalid number of wire objects");
        else
        {
            for (int i = 0; i < wireDraggables.Length; ++i)
            {
                wireDraggables[i].InitializeWireDraggable(i);
                wireTails[i].InitializeWireTail();
            }
        }

        foreach (var node in wireNodes)
        {
            node.InitializeWireNode();
        }
    }

    public void OnMoveWire()
    {
        if (CheckSolution())
        {
            IsComplete = true;
            OnComplete();
        }
    }

    private bool CheckSolution()
    {
        foreach (WirePuzzleTail wpt in wireTails)
        {
            if (!wpt.IsMatched) return false;
        }

        foreach (WirePuzzleNode wpn in wireNodes)
        {
            if (!wpn.IsMatched) return false;
        }

        return true;
    }
}
