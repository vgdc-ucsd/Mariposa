using UnityEngine;

public class BlockPuzzle : Puzzle
{
    public static BlockPuzzle Instance;
    public int GridWidth = 5;
    public int GridHeight = 4;

    private BlockPuzzleBlock[,] grid;
    
    void Awake()
    {
        if(Instance == null) Instance = this;
        else
        {
            Debug.LogWarning("Tried to create more than one instance of the BlockPuzzle singleton!");
            Destroy(this);
        }

        grid = new BlockPuzzleBlock[GridWidth, GridHeight];
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        BlockPuzzleBlock[] blocks = FindObjectsByType<BlockPuzzleBlock>(FindObjectsSortMode.None);
        foreach(BlockPuzzleBlock block in blocks)
        {
            SetBlockInGrid(block, block.position);
        }
    }

    public bool CanMove(BlockPuzzleBlock block, Vector2Int direction)
    {
        Vector2Int newPosition = block.position + direction;

        for (int dx = 0; dx < block.size.x; dx++)
        {
            for (int dy = 0; dy < block.size.y; dy++)
            {
                int x = newPosition.x + dx;
                int y = newPosition.y + dy;

                if (x < 0 || x >= GridWidth || y < 0 || y >= GridHeight || (grid[x, y] != null && grid[x, y] != block))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void MoveBlock(BlockPuzzleBlock block, Vector2Int direction)
    {
        if (CanMove(block, direction))
        {
            // Remove block from old position
            ClearBlockFromGrid(block);

            // Move block to new position
            block.position += direction;
            block.transform.position = GridToWorldPosition(block.position);

            // Update grid with new position
            SetBlockInGrid(block, block.position);
        }
    }

    public void ClearBlockFromGrid(BlockPuzzleBlock block)
    {
        for (int dx = 0; dx < block.size.x; dx++)
        {
            for (int dy = 0; dy < block.size.y; dy++)
            {
                grid[block.position.x + dx, block.position.y + dy] = null;
            }
        }
    }

    public void SetBlockInGrid(BlockPuzzleBlock block, Vector2Int newPosition)
    {
        for (int dx = 0; dx < block.size.x; dx++)
        {
            for (int dy = 0; dy < block.size.y; dy++)
            {
                grid[newPosition.x + dx, newPosition.y + dy] = block;
            }
        }
    }

    public Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x, gridPosition.y, 0);
    }
}
