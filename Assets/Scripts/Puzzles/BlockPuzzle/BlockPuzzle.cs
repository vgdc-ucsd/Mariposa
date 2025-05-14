using UnityEngine;
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

    public BlockPuzzleSlot HoveredSlot = null;
    private BlockPuzzleBlock[,] grid;
    private BlockPuzzleSlot[,] slots;

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
    }

    void OnEnable()
    {
        BlockPuzzleBlock[] blocks = GetComponentsInChildren<BlockPuzzleBlock>();
        foreach (BlockPuzzleBlock block in blocks) block.InitializeBlock();
    }

    public RectTransform GetSlotTransformAtPosition(Vector2Int gridPos)
    {
        return slots[gridPos.x, gridPos.y].GetComponent<RectTransform>();
    }

    public void SetBlockInGrid(BlockPuzzleBlock block, Vector2Int newGridPos)
    {
        foreach (Vector2Int offset in block.Cells)
        {
            Vector2Int cellPos = new Vector2Int(newGridPos.x + offset.x, newGridPos.y + offset.y);
            grid[cellPos.x, cellPos.y] = block;
        }

        RectTransform slotRectTransform = GetSlotTransformAtPosition(newGridPos);
        float cellDiameter = SlotContainer.cellSize.x;  // Assumes cell width and height are the same
        Vector2 worldPos = new Vector2(
            slotRectTransform.position.x - cellDiameter + block.Size.x * cellDiameter,
            slotRectTransform.position.y - cellDiameter + block.Size.y * cellDiameter
        );
        block.SetPosition(worldPos);
        block.GridPos = newGridPos;

        if (CheckSolution()) FinishPuzzle();
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

    public void ClearBlockFromGrid(BlockPuzzleBlock block)
    {
        foreach (Vector2Int offset in block.Cells)
        {
            Vector2Int cellPos = new Vector2Int(block.GridPos.x + offset.x, block.GridPos.y + offset.y);
            grid[cellPos.x, cellPos.y] = null;
        }
        // PrintGridState();
    }

    public bool CheckSolution()
    {
        // PrintGridState();
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
}
