using UnityEngine;

public class WirePuzzle : Puzzle
{
    public static WirePuzzle Instance;
    [Tooltip("Minimum number of wires, inclusive")]
    public int MinWiresCount;
    [Tooltip("Maximum number of wires, exclusive")]
    public int MaxWiresCount;
    [Range(1, 10)]
    public float VerticalSpacing;
    [Range(1, 6)]
    public float HorizontalSpacing;
    public GameObject wireHeadPrefab;
    public GameObject wireTailPrefab;
    public bool IsComplete;
    
    private WirePuzzleDraggable[] wireHeads;
    private GameObject[] wireTailObjects;
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
        wireTailObjects = new GameObject[wiresCount];
        InitializeWires();
    }

    private void InitializeWires() 
    {
        CreateWire(Color.red);
        CreateWire(Color.yellow);
        CreateWire(Color.green);
        CreateWire(Color.blue);

        RandomizeTailPositions();
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

        float posX = CalcWirePositionX(wireIncrementer);
        float posY = VerticalSpacing / 2f;
        wireHeadGO.transform.position = new Vector3(posX, posY, 0);
        wireTailGO.transform.position = new Vector3(posX, posY * -1, 1);

        wireHead.InitializeWireHead(wireIncrementer, wiresCount);
        wireTail.InitializeWireTail();

        wireHeads[wireIncrementer] = wireHead;
        wireTailObjects[wireIncrementer] = wireTailGO;

        wireIncrementer++;
    }

    private void RandomizeTailPositions()
    {
        for (int i = 0; i < wiresCount; i++)
        {
            GameObject temp = wireTailObjects[i];
            int rand = Random.Range(i, wiresCount);
            wireTailObjects[i] = wireTailObjects[rand];
            wireTailObjects[rand] = temp;
            wireTailObjects[i].transform.position = new Vector3(CalcWirePositionX(i), VerticalSpacing / 2f * -1, 1);
            wireTailObjects[rand].transform.position = new Vector3(CalcWirePositionX(rand), VerticalSpacing / 2f * -1, 1);
        }
    }

    private float CalcWirePositionX(int index)
    {
        return ((wiresCount - 1) / 2f - index) * HorizontalSpacing;
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
        foreach (WirePuzzleDraggable wpd in wireHeads)
        {
            if (!wpd.IsMatched) return false;
        }
        return true;
    }
}
