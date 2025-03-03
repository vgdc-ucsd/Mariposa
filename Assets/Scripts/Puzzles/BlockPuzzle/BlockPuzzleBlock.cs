using UnityEngine;

public class BlockPuzzleBlock : MonoBehaviour
{
    private const float BLOCK_SIZE_PADDING = 0.05f;
    public Vector2Int Size;
    public Vector2Int Position;
    public Color Color;

    private Vector3 mouseOffset;
    private bool isDragging = false;
    private Vector2Int originalPosition;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    private BlockPreview preview;

    public void InitializeBlock()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.color = Color;
        boxCollider2D = GetComponent<BoxCollider2D>();

        GameObject previewObj = Instantiate(BlockPuzzle.Instance.previewPrefab, transform.parent);
        preview = previewObj.GetComponent<BlockPreview>();
        preview.SetSize(Size);
        preview.SetSprite(spriteRenderer.sprite, spriteRenderer.color);

        if (spriteRenderer != null && boxCollider2D != null) AdjustVisualSize();
        else Debug.LogWarning("SpriteRenderer not found on child object or BoxCollider2D not found on object.");
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
        if (BlockPuzzle.Instance.CanMove(this, Vector2Int.zero))
        {
            isDragging = true;
            mouseOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            originalPosition = Position;
            
            BlockPuzzle.Instance.ClearBlockFromGrid(this);
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
            
            bool isValidPosition = BlockPuzzle.Instance.IsPositionValidForBlock(nearestGridPos, Size, this);
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
            
            if (BlockPuzzle.Instance.IsPositionValidForBlock(targetPosition, Size, this))
            {
                Position = targetPosition;
                transform.position = BlockPuzzle.Instance.GridToWorldPosition(Position);
                BlockPuzzle.Instance.SetBlockInGrid(this, Position);
                
                if (BlockPuzzle.Instance.CheckSolution())
                {
                    BlockPuzzle.Instance.IsComplete = true;
                    BlockPuzzle.Instance.OnComplete();
                }
            }
            else
            {
                Position = originalPosition;
                transform.position = BlockPuzzle.Instance.GridToWorldPosition(Position);
                BlockPuzzle.Instance.SetBlockInGrid(this, Position);
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

    private void AdjustVisualSize()
    {
        if (spriteRenderer != null && boxCollider2D != null)
        {
            spriteRenderer.transform.localScale = new Vector3(Size.x - BLOCK_SIZE_PADDING, Size.y - BLOCK_SIZE_PADDING, 1);
            Vector3 adjustedPosition = new Vector3((Size.x - 1f) / 2f, (Size.y - 1f) / 2f, 0);
            spriteRenderer.transform.localPosition = adjustedPosition;
            
            boxCollider2D.size = new Vector2(Size.x, Size.y);
            boxCollider2D.offset = new Vector2((Size.x - 1f) / 2f, (Size.y - 1f) / 2f);
        }
    }
}
