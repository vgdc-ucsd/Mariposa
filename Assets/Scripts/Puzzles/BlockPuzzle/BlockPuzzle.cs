using UnityEngine;

public class BlockPuzzle : MonoBehaviour
{
    public static BlockPuzzle Instance;
    public int GridWidth = 5;
    public int GridHeight = 4;
    public GameObject blockPrefab;

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
        InitializeGrid();
    }

    // Create the grid and instantiate blocks
    private void InitializeGrid()
    {
        // Example: Instantiate a few blocks on the grid (you can adjust positions and sizes)
        for (int i = 0; i < GridWidth; i++)
        {
            for (int j = 0; j < GridHeight; j++)
            {
                GameObject blockGO = Instantiate(blockPrefab);
                BlockPuzzleBlock block = blockGO.GetComponent<BlockPuzzleBlock>();

                block.position = new Vector2Int(i, j);  // Set the block's grid position
                block.size = new Vector2Int(1, 1);      // Default to 1x1 blocks
                blockGO.transform.position = GridToWorldPosition(block.position);  // Convert grid position to world position

                SetBlockInGrid(block, block.position);  // Update grid with the block
            }
        }
    }

    // Convert a grid position to world position
    public Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x, gridPosition.y, 0);
    }

    // Set the block in the grid (2D array)
    public void SetBlockInGrid(BlockPuzzleBlock block, Vector2Int newPosition)
    {
        grid[newPosition.x, newPosition.y] = block;
    }

    // Check if a block can move to a new position
    public bool CanMove(BlockPuzzleBlock block, Vector2Int direction)
    {
        Vector2Int newPosition = block.position + direction;
        if (newPosition.x < 0 || newPosition.x >= GridWidth || newPosition.y < 0 || newPosition.y >= GridHeight)
        {
            return false;  // Can't move outside the grid
        }
        return true;
    }
}
