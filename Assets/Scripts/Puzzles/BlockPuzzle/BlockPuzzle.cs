using UnityEngine;
using UnityEngine.SceneManagement;

public class BlockPuzzle : Puzzle
{
    public static BlockPuzzle Instance;
    public int GridWidth = 7;
    public int GridHeight = 7;
    public float GridScale = 0.85f;
    public GameObject gridVisualizerPrefab;
    public bool IsComplete;

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
        Instantiate(gridVisualizerPrefab, transform);
        BlockPuzzleBlock[] blocks = GetComponentsInChildren<BlockPuzzleBlock>();
        foreach (BlockPuzzleBlock block in blocks) block.InitializeBlock();
    }

    public Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        float offsetX = -GridWidth / 2f + 0.5f;
        float offsetY = -GridHeight / 2f + 0.5f;
        return new Vector3(
            (gridPosition.x + offsetX) * GridScale,
            (gridPosition.y + offsetY) * GridScale,
            0
        );
    }

    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        float offsetX = GridWidth / 2f - 0.5f;
        float offsetY = GridHeight / 2f - 0.5f;
        return new Vector2Int(
            Mathf.RoundToInt(worldPosition.x + offsetX),
            Mathf.RoundToInt(worldPosition.y + offsetY)
        );
    }

    public void SetBlockInGrid(BlockPuzzleBlock block, Vector2Int newPosition)
    {
        foreach (Vector2Int offset in block.Cells)
        {
            grid[newPosition.x + offset.x, newPosition.y + offset.y] = block;
        }

        if (CheckSolution())
        {
            IsComplete = true;
            OnComplete();
            SceneManager.LoadScene(0); // TODO remove after EOQ social
        }
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

        if (!IsPositionInGridForBlock(position, block))
        {
            return false;
        }

        foreach (Vector2Int offset in block.Cells)
        {
            int x = position.x + offset.x;
            int y = position.y + offset.y;
            if (grid[x, y] != null && grid[x, y] != block) return false;
        }

        return true;
    }

    public bool IsPositionInGridForBlock(Vector2Int position, BlockPuzzleBlock block)
    {
        return position.x >= 0 && position.x + block.Size.x <= GridWidth && 
            position.y >= 0 && position.y + block.Size.y <= GridHeight;
    }

    public void ClearBlockFromGrid(BlockPuzzleBlock block)
    {
        foreach (Vector2Int offset in block.Cells)
        {
            grid[block.Position.x + offset.x, block.Position.y + offset.y] = null;
        }
    }

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
}
