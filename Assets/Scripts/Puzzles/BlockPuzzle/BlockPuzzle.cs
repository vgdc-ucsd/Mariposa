using UnityEngine;

public class BlockPuzzle : Puzzle
{
    public static BlockPuzzle Instance;
    public int GridWidth = 5;
    public int GridHeight = 4;
    public int SolutionColumn = 2;
    public GameObject blockPrefab;
    public GameObject previewPrefab;
    public GameObject gridVisualizerPrefab;
    public bool IsComplete;

    private BlockPuzzleBlock[,] grid;
    private int blockCount = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Debug.LogWarning("Tried to create more than one instance of the BlockPuzzle singleton!");
            Destroy(this);
        }

        grid = new BlockPuzzleBlock[GridWidth, GridHeight];
        Instantiate(gridVisualizerPrefab, transform);
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        // Example: Instantiate blocks
        CreateBlockAtPosition(new Vector2Int(0, 1), new Vector2Int(2, 1), Color.cyan);
        CreateBlockAtPosition(new Vector2Int(0, 2), new Vector2Int(1, 2), Color.green);
        CreateBlockAtPosition(new Vector2Int(1, 2), new Vector2Int(2, 2), Color.yellow);
        CreateBlockAtPosition(new Vector2Int(2, 0), new Vector2Int(2, 2), Color.yellow);
        CreateBlockAtPosition(new Vector2Int(3, 2), new Vector2Int(2, 1), Color.cyan);
        CreateBlockAtPosition(new Vector2Int(4, 0), new Vector2Int(1, 1), Color.magenta);
        CreateBlockAtPosition(new Vector2Int(4, 1), new Vector2Int(1, 1), Color.magenta);
    }

    private void CreateBlockAtPosition(Vector2Int position, Vector2Int size, Color color)
    {
        GameObject blockGO = Instantiate(blockPrefab);
        blockGO.name = $"Block {blockCount++}";
        BlockPuzzleBlock block = blockGO.GetComponent<BlockPuzzleBlock>();

        block.Position = position;
        block.Size = size;
        block.Color = color;
        
        // Convert to world position
        blockGO.transform.position = GridToWorldPosition(block.Position);

        block.InitializeBlock();
        SetBlockInGrid(block, block.Position);
    }

    public Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        // Convert from grid coordinates to world coordinates
        // Add 0.5f to center within grid cells
        float offsetX = -GridWidth / 2f + 0.5f;
        float offsetY = -GridHeight / 2f + 0.5f;
        return new Vector3(
            gridPosition.x + offsetX,
            gridPosition.y + offsetY,
            0
        );
    }

    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        // Convert from world coordinates to grid coordinates
        // Subtract 0.5f to account for cell centering
        float offsetX = GridWidth / 2f - 0.5f;
        float offsetY = GridHeight / 2f - 0.5f;
        return new Vector2Int(
            Mathf.RoundToInt(worldPosition.x + offsetX),
            Mathf.RoundToInt(worldPosition.y + offsetY)
        );
    }

    public void SetBlockInGrid(BlockPuzzleBlock block, Vector2Int newPosition)
    {
        for (int dx = 0; dx < block.Size.x; dx++)
        {
            for (int dy = 0; dy < block.Size.y; dy++)
            {
                grid[newPosition.x + dx, newPosition.y + dy] = block;
            }
        }
    }

    public bool CanMove(BlockPuzzleBlock block, Vector2Int direction)
    {
        if (IsComplete) return false;

        Vector2Int newPosition = block.Position + direction;

        // Ensure the new position is within bounds of the grid
        if (newPosition.x < 0 || newPosition.x + block.Size.x > GridWidth || newPosition.y < 0 || newPosition.y + block.Size.y > GridHeight)
        {
            return false;  // Out of bounds
        }

        // Check if the new space is free (not occupied by other blocks)
        for (int dx = 0; dx < block.Size.x; dx++)
        {
            for (int dy = 0; dy < block.Size.y; dy++)
            {
                int x = newPosition.x + dx;
                int y = newPosition.y + dy;

                if (grid[x, y] != null && grid[x, y] != block)
                {
                    return false;  // Space is occupied
                }
            }
        }
        return true;  // The block can move
    }

    public bool CanMoveInDirection(BlockPuzzleBlock block, Vector2Int direction, out int maxSteps)
    {
        maxSteps = 0;
        if (direction == Vector2Int.zero) return true;
        
        while (true)
        {
            Vector2Int newPosition = block.Position + (direction * (maxSteps + 1));

            if (newPosition.x < 0 || newPosition.x + block.Size.x > GridWidth || 
                newPosition.y < 0 || newPosition.y + block.Size.y > GridHeight)
            {
                break;
            }

            bool positionClear = true;
            for (int dx = 0; dx < block.Size.x; dx++)
            {
                for (int dy = 0; dy < block.Size.y; dy++)
                {
                    int x = newPosition.x + dx;
                    int y = newPosition.y + dy;
                    
                    if (grid[x, y] != null && grid[x, y] != block)
                    {
                        positionClear = false;
                        break;
                    }
                }
                if (!positionClear) break;
            }
            
            if (!positionClear) break;
            maxSteps++;
        }
        
        return maxSteps > 0;
    }

    public void MoveBlock(BlockPuzzleBlock block, Vector2Int direction, int steps = 1)
    {
        // Clear block from old position
        ClearBlockFromGrid(block);
        
        // Move the block to the new position
        block.Position += direction * steps;
        block.transform.position = GridToWorldPosition(block.Position);
        
        // Update grid with new position
        SetBlockInGrid(block, block.Position);

        // Check if puzzle is solved after each move
        if (CheckSolution())
        {
            IsComplete = true;
            OnComplete();
        }
    }

    public void ClearBlockFromGrid(BlockPuzzleBlock block)
    {
        // Reset all positions occupied by the block to null
        for (int dx = 0; dx < block.Size.x; dx++)
        {
            for (int dy = 0; dy < block.Size.y; dy++)
            {
                grid[block.Position.x + dx, block.Position.y + dy] = null;
            }
        }
    }

    private bool CheckSolution()
    {
        for (int i = 0; i < GridHeight; ++i)
        {
            if(grid[SolutionColumn, i] != null) return false;
        }
        return true;
    }
}
