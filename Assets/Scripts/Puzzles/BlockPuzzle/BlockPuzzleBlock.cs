using UnityEngine;

public class BlockPuzzleBlock : MonoBehaviour
{
    public Vector2Int Position;
    public Vector2Int Size;
    public Vector2Int[] Cells;
    public Color Color;
    public GameObject PreviewPrefab;
    public bool IsFixed = false;

    private Vector3 mouseOffset;
    private bool isDragging = false;
    private Vector2Int originalPosition;

    private SpriteRenderer spriteRenderer;
    private BlockPreview preview;

    public void InitializeBlock()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.color = Color;

        // For debug, remove later
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in sprites) sprite.color = Color;

        if (PreviewPrefab != null)
        {
            GameObject previewObj = Instantiate(PreviewPrefab, transform.parent);
            preview = previewObj.GetComponent<BlockPreview>();
            preview.SetSprite(spriteRenderer.sprite, spriteRenderer.color);
        }

        BlockPuzzle.Instance.SetBlockInGrid(this, Position);
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
        if (BlockPuzzle.Instance.CanMove(this, Vector2Int.zero) && !IsFixed)
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
}
