using UnityEngine;

public class BlockPuzzle : Puzzle
{
    public const float BLOCK_GRID_SCALE = 1f;
    public static BlockPuzzle Instance;
    public int GridWidth = 7;
    public int GridHeight = 7;
    public GameObject gridVisualizerPrefab;
    public bool IsComplete;
    public RectTransform gridContainer; // Reference to the grid container UI element

    private BlockPuzzleBlock[,] grid;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Debug.LogWarning("Tried to create more than one instance of the BlockPuzzle singleton!");
            Destroy(this);
        }

        grid = new BlockPuzzleBlock[GridWidth, GridHeight];
        BlockPuzzleBlock[] blocks = GetComponentsInChildren<BlockPuzzleBlock>();
        foreach (BlockPuzzleBlock block in blocks) block.InitializeBlock();
    }

    // Calculate world position for a grid cell - used for UI positioning
    public Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        // Calculate the cell size based on the grid container
        float cellWidth = gridContainer.rect.width / GridWidth;
        float cellHeight = gridContainer.rect.height / GridHeight;

        // Calculate the local position within the grid container
        // Position is centered in the cell
        float localX = (gridPosition.x * cellWidth) + (cellWidth / 2) - (gridContainer.rect.width / 2);
        float localY = (gridPosition.y * cellHeight) + (cellHeight / 2) - (gridContainer.rect.height / 2);

        // Convert from local position to world position
        Vector3 worldPosition = gridContainer.TransformPoint(new Vector3(localX, localY, 0));
        
        return worldPosition;
    }

    // Convert a world position to grid coordinates
    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        // Convert world position to local position within grid container
        Vector3 localPosition = gridContainer.InverseTransformPoint(worldPosition);
        
        // Calculate the cell size
        float cellWidth = gridContainer.rect.width / GridWidth;
        float cellHeight = gridContainer.rect.height / GridHeight;

        // Calculate grid position
        float offsetX = gridContainer.rect.width / 2;
        float offsetY = gridContainer.rect.height / 2;
        
        int gridX = Mathf.FloorToInt((localPosition.x + offsetX) / cellWidth);
        int gridY = Mathf.FloorToInt((localPosition.y + offsetY) / cellHeight);

        // Clamp to valid grid range
        gridX = Mathf.Clamp(gridX, 0, GridWidth - 1);
        gridY = Mathf.Clamp(gridY, 0, GridHeight - 1);

        return new Vector2Int(gridX, gridY);
    }

    // Set block in the grid data structure
    public void SetBlockInGrid(BlockPuzzleBlock block, Vector2Int newPosition)
    {
        foreach (Vector2Int offset in block.Cells)
        {
            Vector2Int cellPos = new Vector2Int(newPosition.x + offset.x, newPosition.y + offset.y);
            if (IsPositionInGrid(cellPos))
            {
                grid[cellPos.x, cellPos.y] = block;
            }
        }

        if (CheckSolution()) FinishPuzzle();
    }

    // Check if a block can move to the specified position
    public bool CanMove(BlockPuzzleBlock block, Vector2Int direction)
    {
        if (IsComplete) return false;

        Vector2Int newPosition = block.Position + direction;
        return IsPositionValidForBlock(newPosition, block);
    }

    // Check if the position is valid for placing a block
    public bool IsPositionValidForBlock(Vector2Int position, BlockPuzzleBlock block)
    {
        if (IsComplete) return false;

        if (!IsPositionInGridForBlock(position, block))
        {
            return false;
        }

        foreach (Vector2Int offset in block.Cells)
        {
            Vector2Int cellPos = new Vector2Int(position.x + offset.x, position.y + offset.y);
            if (grid[cellPos.x, cellPos.y] != null && grid[cellPos.x, cellPos.y] != block) 
                return false;
        }

        return true;
    }

    // Check if a single cell position is in the grid
    private bool IsPositionInGrid(Vector2Int position)
    {
        return position.x >= 0 && position.x < GridWidth && 
               position.y >= 0 && position.y < GridHeight;
    }

    // Check if the entire block is within the grid
    public bool IsPositionInGridForBlock(Vector2Int position, BlockPuzzleBlock block)
    {
        foreach (Vector2Int offset in block.Cells)
        {
            Vector2Int cellPos = new Vector2Int(position.x + offset.x, position.y + offset.y);
            if (!IsPositionInGrid(cellPos))
                return false;
        }
        return true;
    }

    // Clear a block from the grid data structure
    public void ClearBlockFromGrid(BlockPuzzleBlock block)
    {
        foreach (Vector2Int offset in block.Cells)
        {
            Vector2Int cellPos = new Vector2Int(block.Position.x + offset.x, block.Position.y + offset.y);
            if (IsPositionInGrid(cellPos))
            {
                grid[cellPos.x, cellPos.y] = null;
            }
        }
    }

    // Check if all grid cells are filled (puzzle solved)
    public bool CheckSolution()
    {
        for (int i = 0; i < GridWidth; ++i)
        {
            for (int j = 0; j < GridHeight; ++j)
            {
                if (grid[i,j] == null) return false;
            }
        }
        return true;
    }

    // Handle puzzle completion
    private void FinishPuzzle()
    {
        IsComplete = true;
        // OnComplete();
    }
}
