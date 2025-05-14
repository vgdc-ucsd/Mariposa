using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class BlockPuzzle : Puzzle
{
    public const float BLOCK_GRID_SCALE = 1f;
    public static BlockPuzzle Instance;
    public int GridWidth = 7;
    public int GridHeight = 7;
    public GameObject gridVisualizerPrefab;
    public bool IsComplete;
    public RectTransform gridContainer;
    public GridLayoutGroup SlotContainer;
    public GameObject SlotPrefab;

    [HideInInspector] public float CellDiameter;
    [HideInInspector] public BlockPuzzleSlot HoveredSlot = null;
    private BlockPuzzleBlock[,] grid;
    private BlockPuzzleSlot[,] slots;
    BlockPuzzleBlock[] blocks;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Debug.LogWarning("Tried to create more than one instance of the BlockPuzzle singleton!");
            Destroy(this);
        }

        grid = new BlockPuzzleBlock[GridWidth, GridHeight];
        slots = new BlockPuzzleSlot[GridWidth, GridHeight];

        SlotContainer.cellSize = new Vector2(gridContainer.rect.width / GridWidth, gridContainer.rect.height / GridHeight);
        CellDiameter = SlotContainer.cellSize.x;
        for (int i = 0; i < GridHeight; i++)
        {
            for (int j = 0; j < GridWidth; j++)
            {
                BlockPuzzleSlot temp = Instantiate(SlotPrefab, SlotContainer.transform).GetComponent<BlockPuzzleSlot>();
                temp.GridPos = new Vector2Int(j, i);
                temp.gameObject.name = $"Slot_{j}_{i}";
                slots[j,i] = temp;
            }
        }

        blocks = GetComponentsInChildren<BlockPuzzleBlock>();
    }

    void OnEnable()
    {
        StartCoroutine(DelayInitializeBlocks());
    }

    IEnumerator DelayInitializeBlocks()
    {
        yield return new WaitForEndOfFrame();
        foreach (BlockPuzzleBlock block in blocks) block.InitializeBlock();
    }

    public RectTransform GetSlotTransformAtPosition(Vector2Int gridPos)
    {
        return slots[gridPos.x, gridPos.y].GetComponent<RectTransform>();
    }

    public bool IsPositionValidForBlock(Vector2Int gridPos, BlockPuzzleBlock block)
    {
        if (IsComplete) return false;

        foreach (Vector2Int offset in block.Cells)
        {
            Vector2Int cellPos = new Vector2Int(gridPos.x + offset.x, gridPos.y + offset.y);
            if (cellPos.x < 0 || cellPos.x >= GridWidth || cellPos.y < 0 || cellPos.y >= GridHeight) return false;
            if (grid[cellPos.x, cellPos.y] != null) return false;
        }

        return true;
    }

    public void SetBlockInGrid(BlockPuzzleBlock block, Vector2Int newGridPos)
    {
        Debug.Log($"Setting {block.gameObject.name} in {newGridPos}");
        foreach (Vector2Int offset in block.Cells)
        {
            Vector2Int cellPos = new Vector2Int(newGridPos.x + offset.x, newGridPos.y + offset.y);
            grid[cellPos.x, cellPos.y] = block;
        }

        RectTransform slotRectTransform = GetSlotTransformAtPosition(newGridPos);
        Vector2 worldPos = new Vector2(
            slotRectTransform.position.x + (block.Size.x - 1) * CellDiameter,
            slotRectTransform.position.y + (block.Size.y - 1) * CellDiameter
        );
        block.SetPosition(worldPos);
        block.GridPos = newGridPos;

        if (CheckSolution()) FinishPuzzle();
    }

    public void ClearBlockFromGrid(BlockPuzzleBlock block)
    {
        Debug.Log($"Clearing {block.gameObject.name}");
        foreach (Vector2Int offset in block.Cells)
        {
            Vector2Int cellPos = new Vector2Int(block.GridPos.x + offset.x, block.GridPos.y + offset.y);
            if (cellPos.x < 0 || cellPos.x >= GridWidth || cellPos.y < 0 || cellPos.y >= GridHeight) continue;
            grid[cellPos.x, cellPos.y] = null;
        }
        PrintGridState();
    }

    public bool CheckSolution()
    {
        PrintGridState();
        for (int i = 0; i < GridWidth; ++i)
        {
            for (int j = 0; j < GridHeight; ++j)
            {
                if (grid[i,j] == null) return false;
            }
        }
        return true;
    }

    private void FinishPuzzle()
    {
        IsComplete = true;
        OnComplete();
    }

    // TODO: debug, remove
    private void PrintGridState()
    {
        string output = "\n";
        for (int i = GridHeight - 1; i >= 0; i--)
        {
            for (int j = 0; j < GridWidth; j++)
            {
                if (grid[j,i] != null) output += "#";
                else output += "-";
            }
            output += "\n";
        }
        Debug.Log(output);
    }

    // TODO: debug, remove
    public void DebugSelectSlot(Vector2Int gridPos)
    {
        slots[gridPos.x, gridPos.y].GetComponent<Image>().color = Color.red;
    }
}
