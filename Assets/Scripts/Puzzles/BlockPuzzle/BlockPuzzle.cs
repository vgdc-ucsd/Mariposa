using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlockPuzzle : Puzzle
{
    public const float BLOCK_GRID_SCALE = 1f;
    public static BlockPuzzle Instance;
    public int GridWidth = 7;
    public int GridHeight = 7;
    public RectTransform gridContainer;
    public GridLayoutGroup SlotContainer;
    public GameObject SlotPrefab;

    public delegate void OnStartDragBlock();
    public static OnStartDragBlock onStartDragBlock;
    public delegate void OnEndDragBlock();
    public static OnEndDragBlock onEndDragBlock;

    [HideInInspector] public float CellDiameter;
    [HideInInspector] public BlockPuzzleSlot HoveredSlot = null;
    private BlockPuzzleBlock[,] grid;
    private BlockPuzzleSlot[,] slots;
    BlockPuzzleBlock[] blocks;

    public Dialogue dialogue, radioDialogue; // played when puzzle is completed
    [SerializeField] private TutorialLever lever;

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
        try
        {
            return slots[gridPos.x, gridPos.y].GetComponent<RectTransform>();
        }
        catch
        {
            return null;
        }
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

    public Vector2 GetWorldPositionForBlock(BlockPuzzleBlock block, RectTransform rect)
    {
        return new Vector2(
            rect.anchoredPosition.x + (-4f + block.Size.x / 2f) * CellDiameter,
            rect.anchoredPosition.y + (3f + block.Size.y / 2f) * CellDiameter
        );
    }

    public void SetBlockInGrid(BlockPuzzleBlock block, Vector2Int newGridPos)
    {
        foreach (Vector2Int offset in block.Cells)
        {
            Vector2Int cellPos = new Vector2Int(newGridPos.x + offset.x, newGridPos.y + offset.y);
            grid[cellPos.x, cellPos.y] = block;
        }

        RectTransform slotRectTransform = GetSlotTransformAtPosition(newGridPos);
        Vector2 worldPos = GetWorldPositionForBlock(block, slotRectTransform);
        block.SetPosition(worldPos);
        block.GridPos = newGridPos;

        if (CheckSolution()) FinishPuzzle();
    }

    public void ClearBlockFromGrid(BlockPuzzleBlock block)
    {
        foreach (Vector2Int offset in block.Cells)
        {
            Vector2Int cellPos = new Vector2Int(block.GridPos.x + offset.x, block.GridPos.y + offset.y);
            if (cellPos.x < 0 || cellPos.x >= GridWidth || cellPos.y < 0 || cellPos.y >= GridHeight) continue;
            grid[cellPos.x, cellPos.y] = null;
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

    private void FinishPuzzle()
    {
        if (lever.SwitchToggled)
        {
            DialogueManager.Instance.PlayDialogue(dialogue, () =>
            {
                LevelManager.Instance.GoToNextSublevel();
                DialogueManager.Instance.PlayDialogue(radioDialogue);
            });
        }
        OnComplete();
        
    }
}
