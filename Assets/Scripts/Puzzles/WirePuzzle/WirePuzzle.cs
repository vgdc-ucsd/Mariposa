using System.Collections.Generic;
using UnityEngine;

public class WirePuzzle : Puzzle
{
    public static WirePuzzle Instance;
    [Tooltip("Minimum number of wires, inclusive")]
    public int MinWiresCount;
    [Tooltip("Maximum number of wires, exclusive")]
    public int MaxWiresCount;
    [Range(1, 3)]
    public float VerticalSpacing;
    [Range(1, 10)]
    public float HorizontalSpacing;
    public GameObject wireHeadPrefab;
    public GameObject wireTailPrefab;
    public bool IsComplete;
    
    private WirePuzzleDraggable[] wireHeads;
    private int wiresCount;
    private int wireIncrementer;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Debug.LogWarning("Tried to create more than one instance of the WirePuzzle singleton!");
            Destroy(this);
        }

        if (MaxWiresCount <= MinWiresCount) {
            Debug.LogWarning("Invalid MaxWireCount");
            return;
        }

        wiresCount = Random.Range(MinWiresCount, MaxWiresCount); // MaxWiresCount is exclusive
        wireHeads = new WirePuzzleDraggable[wiresCount];
        InitializeWires();
    }

    private void InitializeWires() 
    {
        CreateWire(Color.red);
        CreateWire(Color.yellow);
        CreateWire(Color.green);
        CreateWire(Color.blue);
    }

    private void CreateWire(Color color)
    {
        WirePuzzleWire wire = new(color, wireIncrementer);
        
        GameObject wireHeadGO = Instantiate(wireHeadPrefab, transform);
        WirePuzzleDraggable wireHead = wireHeadGO.GetComponentInChildren<WirePuzzleDraggable>();
        wireHeadGO.name = $"Wire Head {wireIncrementer}";

        GameObject wireTailGO = Instantiate(wireTailPrefab, transform);
        WirePuzzleTail wireTail = wireTailGO.GetComponent<WirePuzzleTail>();
        wireTailGO.name = $"Wire Tail {wireIncrementer}";

        wireHead.Wire = wire;
        wireTail.Wire = wire;
        
        Debug.Log("Hello world");

        float posX = HorizontalSpacing / 2f;
        float posY = CalcWirePositionY(wireIncrementer);
        wireHeadGO.transform.position = new Vector2(posX * -1, posY);
        wireTailGO.transform.position = new Vector2(posX, posY);

        wireHead.InitializeWireHead();
        wireTail.InitializeWireTail();

        wireHeads[wireIncrementer] = wireHead;

        wireIncrementer++;
    }

    private float CalcWirePositionY(int index)
    {
        return ((wiresCount - 1) / 2f - index) * VerticalSpacing;
    }

    public void MoveWire()
    {
        // TODO: move wires

        if (CheckSolution())
        {
            IsComplete = true;
            OnComplete();
        }
    }

    private bool CheckSolution()
    {
        foreach (WirePuzzleDraggable wph in wireHeads)
        {
            if (!wph.IsMatched) return false;
        }
        return true;
    }
}
