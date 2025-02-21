using UnityEngine;

public class BlockPuzzle : MonoBehaviour
{
    public static BlockPuzzle Instance;
    public int GridWidth = 5;
    public int GridHeight = 4;
    public GameObject blockPrefab;

    private BlockPuzzleBlock[,] grid; // Store blocks in the grid
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
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        // Example: Instantiate a few blocks at specific grid positions to leave room for testing movement
        CreateBlockAtPosition(0, 0, new Vector2Int(1, 1)); // Block at (0, 0), size 1x1
        CreateBlockAtPosition(1, 0, new Vector2Int(2, 1)); // Block at (1, 0), size 2x1
        CreateBlockAtPosition(4, 2, new Vector2Int(1, 2)); // Block at (4, 2), size 1x2
        CreateBlockAtPosition(2, 1, new Vector2Int(1, 1)); // Block at (2, 1), size 1x1
    }

    private void CreateBlockAtPosition(int x, int y, Vector2Int size)
    {
        GameObject blockGO = Instantiate(blockPrefab);
        blockGO.name = $"Block {blockCount++}";
        BlockPuzzleBlock block = blockGO.GetComponent<BlockPuzzleBlock>();

        block.position = new Vector2Int(x, y);  // Set the block's grid position
        block.size = size;                       // Set the block's size
        blockGO.transform.position = GridToWorldPosition(block.position);

        block.InitializeBlock();
        SetBlockInGrid(block, block.position);  // Update grid with the block
    }

    public Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x, gridPosition.y, 0);
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

    public bool CanMove(BlockPuzzleBlock block, Vector2Int direction)
    {
        Vector2Int newPosition = block.position + direction;

        // Ensure the new position is within bounds of the grid
        if (newPosition.x < 0 || newPosition.x + block.size.x > GridWidth || newPosition.y < 0 || newPosition.y + block.size.y > GridHeight)
        {
            Debug.Log($"Move out of bounds: {newPosition}");
            return false;  // Out of bounds
        }

        // Check if the new space is free (not occupied by other blocks)
        for (int dx = 0; dx < block.size.x; dx++)
        {
            for (int dy = 0; dy < block.size.y; dy++)
            {
                int x = newPosition.x + dx;
                int y = newPosition.y + dy;

                if (grid[x, y] != null && grid[x, y] != block)
                {
                    Debug.Log($"Cannot move {block.name} to ({x}, {y}) - Occupied by {grid[x, y].name}");
                    return false;  // Space is occupied
                }
            }
        }
        return true;  // The block can move
    }


    public void MoveBlock(BlockPuzzleBlock block, Vector2Int direction)
    {
        Debug.Log($"Trying to move {block}");
        if (CanMove(block, direction))
        {
            Debug.Log($"Successfully moved {block}");
            // Clear block from old position in grid
            ClearBlockFromGrid(block);

            // Move the block to the new position
            block.position += direction;
            block.transform.position = GridToWorldPosition(block.position);

            // Update grid with new position
            SetBlockInGrid(block, block.position);
        }
        else
        {
            Debug.LogWarning("Move failed for block at position " + block.position);
        }
    }

    public void ClearBlockFromGrid(BlockPuzzleBlock block)
    {
        Debug.Log($"Clearing grid for {block}");
        // Reset all positions occupied by the block to null
        for (int dx = 0; dx < block.size.x; dx++)
        {
            for (int dy = 0; dy < block.size.y; dy++)
            {
                grid[block.position.x + dx, block.position.y + dy] = null;
            }
        }
    }
}
