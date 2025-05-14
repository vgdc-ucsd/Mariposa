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
                temp.GridPosition = new Vector2Int(j, i);
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

    public RectTransform GetSlotTransformAtPosition(Vector2Int gridPosition)
    {
        return slots[gridPosition.x, gridPosition.y].GetComponent<RectTransform>();
    }

    public void SetBlockInGrid(BlockPuzzleBlock block, Vector2Int newPosition)
    {
        foreach (Vector2Int offset in block.Cells)
        {
            Vector2Int cellPos = new Vector2Int(newPosition.x + offset.x, newPosition.y + offset.y);
            grid[cellPos.x, cellPos.y] = block;
        }

        RectTransform slotRectTransform = GetSlotTransformAtPosition(newPosition);
        Vector2 transformPosition = new Vector2(
            block.Size.x * SlotContainer.cellSize.x / 2 + slotRectTransform.position.x,
            block.Size.y * SlotContainer.cellSize.y / 2 + slotRectTransform.position.y
        );
        block.SetPosition(transformPosition);
        block.Position = newPosition;

        if (CheckSolution()) FinishPuzzle();
    }

    public bool CanMove(BlockPuzzleBlock block, Vector2Int direction)
    {
        if (IsComplete) return false;

        Vector2Int newPosition = block.Position + direction;
        return IsPositionValidForBlock(newPosition, block);
    }

    public bool IsPositionValidForBlock(Vector2Int position, BlockPuzzleBlock block)
    {
        if (IsComplete) return false;

        foreach (Vector2Int offset in block.Cells)
        {
            Vector2Int cellPos = new Vector2Int(position.x + offset.x, position.y + offset.y);
            if (!IsPositionInGrid(cellPos)) return false;
            // if (grid[cellPos.x, cellPos.y] != null && grid[cellPos.x, cellPos.y] != block) return false;
            if (grid[cellPos.x, cellPos.y] != null) return false;
        }

        return true;
    }

    private bool IsPositionInGrid(Vector2Int position)
    {
        return position.x >= 0 && position.x < GridWidth && 
               position.y >= 0 && position.y < GridHeight;
    }

    public void ClearBlockFromGrid(BlockPuzzleBlock block)
    {
        foreach (Vector2Int offset in block.Cells)
        {
            Vector2Int cellPos = new Vector2Int(block.Position.x + offset.x, block.Position.y + offset.y);
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
