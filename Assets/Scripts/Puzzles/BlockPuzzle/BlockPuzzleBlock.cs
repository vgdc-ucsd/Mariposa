using UnityEngine;

public class BlockPuzzleBlock : MonoBehaviour
{
    public Vector2Int Position;
    public Vector2Int Size;
    public Vector2Int[] Cells;
    public GameObject PreviewPrefab;
    public bool IsFixed = false;

    private Vector3 mouseOffset;
    private bool isDragging = false;
    private Vector2Int originalPosition;
    private Vector3 stagingPosition;
    [SerializeField] private bool isInGrid = false;

    private SpriteRenderer spriteRenderer;
    private BlockPreview preview;

    public bool IsDragging => isDragging;

    public void InitializeBlock()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (IsFixed)
        {
            BlockPuzzle.Instance.SetBlockInGrid(this, Position);
            isInGrid = true;
        }
        else
        {
            if (PreviewPrefab != null)
            {
                GameObject previewObj = Instantiate(PreviewPrefab, transform.parent);
                preview = previewObj.GetComponent<BlockPreview>();
                preview.SetSprite(spriteRenderer.sprite, spriteRenderer.color);
            }
            stagingPosition = transform.position;
        }
    }

    private void OnDestroy()
    {
        if (preview != null)
        {
            Destroy(preview.gameObject);
        }
    }

    private void OnMouseDown()
    {
        if ((isInGrid && BlockPuzzle.Instance.CanMove(this, Vector2Int.zero) || !isInGrid) && !IsFixed)
        {
            isDragging = true;
            mouseOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            if (isInGrid)
            {
                originalPosition = Position;
                BlockPuzzle.Instance.ClearBlockFromGrid(this);
            }
        }
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + mouseOffset;
            mousePosition.z = 0;

            Vector2Int nearestGridPos = SnapToGrid(mousePosition);
            Vector3 previewPos = BlockPuzzle.Instance.GridToWorldPosition(nearestGridPos);

            transform.position = mousePosition;

            bool isValidPosition = BlockPuzzle.Instance.IsPositionValidForBlock(nearestGridPos, this);
            if (isValidPosition) ShowPreview(previewPos);
            else ShowPreview(previewPos, false);
        }
    }

    private void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + mouseOffset;
            Vector2Int targetPosition = SnapToGrid(mousePosition);

            if (BlockPuzzle.Instance.IsPositionValidForBlock(targetPosition, this))
            {
                Position = targetPosition;
                transform.position = BlockPuzzle.Instance.GridToWorldPosition(Position);
                BlockPuzzle.Instance.SetBlockInGrid(this, Position);
                isInGrid = true;
            }
            else
            {
                if (BlockPuzzle.Instance.IsPositionInGridForBlock(targetPosition, this) && isInGrid)
                {
                    Position = originalPosition;
                    transform.position = BlockPuzzle.Instance.GridToWorldPosition(Position);
                    BlockPuzzle.Instance.SetBlockInGrid(this, Position);
                }
                else
                {
                    transform.position = stagingPosition;
                    isInGrid = false;
                }
            }

            HidePreview();
        }
    }

    private void ShowPreview(Vector3 position, bool isValid = true)
    {
        if (preview != null)
        {
            preview.SetPosition(position + new Vector3((Size.x - 1f) / 2f, (Size.y - 1f) / 2f, 0));

            if (!isValid) preview.SetSprite(spriteRenderer.sprite, new Color(1f, 0.3f, 0.3f));
            else preview.SetSprite(spriteRenderer.sprite, spriteRenderer.color);

            preview.Show();
        }
    }

    private void HidePreview()
    {
        if (preview != null)
        {
            preview.Hide();
        }
    }

    private Vector2Int SnapToGrid(Vector3 worldPosition)
    {
        return BlockPuzzle.Instance.WorldToGridPosition(worldPosition);
    }
}
