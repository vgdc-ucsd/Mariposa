using UnityEngine;

public class BlockPuzzleBlock : MonoBehaviour
{
    public Vector2Int Size;
    public Vector2Int Position;
    public Color Color;

    private Vector3 mouseOffset;
    private bool isDragging = false;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    private BlockPreview preview;

    public void InitializeBlock()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.color = Color;
        boxCollider2D = GetComponent<BoxCollider2D>();

        // Create preview
        GameObject previewObj = Instantiate(BlockPuzzle.Instance.previewPrefab);
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
        }
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + mouseOffset;
            mousePosition.z = 0;
            
            Vector2Int validPos = GetValidPosition(mousePosition);
            Vector3 constrainedPos = BlockPuzzle.Instance.GridToWorldPosition(validPos);
            
            transform.position = mousePosition;
            ShowPreview(constrainedPos);
        }
    }

    private void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;
            
            // Get the closest valid position based on where the player dragged
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + mouseOffset;
            Vector2Int newPosition = GetValidPosition(mousePosition);
            
            if (newPosition != Position)
            {
                Vector2Int movement = newPosition - Position;
                Vector2Int direction;
                int steps;
                
                if (movement.x != 0)
                {
                    direction = new Vector2Int(movement.x > 0 ? 1 : -1, 0);
                    steps = Mathf.Abs(movement.x);
                }
                else
                {
                    direction = new Vector2Int(0, movement.y > 0 ? 1 : -1);
                    steps = Mathf.Abs(movement.y);
                }
                
                BlockPuzzle.Instance.MoveBlock(this, direction, steps);
            }
            else
            {
                transform.position = BlockPuzzle.Instance.GridToWorldPosition(Position);
            }
            
            // Hide the preview when dragging ends
            HidePreview();
        }
    }

    private void ShowPreview(Vector3 position)
    {
        if (preview != null)
        {
            // Adjust preview position for centered blocks
            preview.SetPosition(position + new Vector3((Size.x - 1f) / 2f, (Size.y - 1f) / 2f, 0));
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

    private Vector2Int GetValidPosition(Vector3 worldPosition)
    {
        Vector2Int targetPos = SnapToGrid(worldPosition);
        Vector2Int movement = targetPos - Position;
        
        // If movement is purely horizontal or vertical
        if (movement.x != 0 && movement.y == 0)
        {
            // Horizontal movement
            Vector2Int direction = new Vector2Int(movement.x > 0 ? 1 : -1, 0);
            int maxSteps;
            if (BlockPuzzle.Instance.CanMoveInDirection(this, direction, out maxSteps))
            {
                int desiredSteps = Mathf.Abs(movement.x);
                // Allow any number of steps up to the maximum
                int actualSteps = Mathf.Min(desiredSteps, maxSteps);
                return Position + (direction * actualSteps);
            }
        }
        else if (movement.x == 0 && movement.y != 0)
        {
            // Vertical movement
            Vector2Int direction = new Vector2Int(0, movement.y > 0 ? 1 : -1);
            int maxSteps;
            if (BlockPuzzle.Instance.CanMoveInDirection(this, direction, out maxSteps))
            {
                int desiredSteps = Mathf.Abs(movement.y);
                // Allow any number of steps up to the maximum
                int actualSteps = Mathf.Min(desiredSteps, maxSteps);
                return Position + (direction * actualSteps);
            }
        }
        
        return Position;
    }

    // Snap block's world position to the nearest grid position
    private Vector2Int SnapToGrid(Vector3 worldPosition)
    {
        return BlockPuzzle.Instance.WorldToGridPosition(worldPosition);
    }

    // Adjust the visual size of the block based on its size
    private void AdjustVisualSize()
    {
        if (spriteRenderer != null && boxCollider2D != null)
        {
            spriteRenderer.transform.localScale = new Vector3(Size.x, Size.y, 1);
            Vector3 adjustedPosition = new Vector3((Size.x - 1f) / 2f, (Size.y - 1f) / 2f, 0);
            spriteRenderer.transform.localPosition = adjustedPosition;
            
            boxCollider2D.size = new Vector2(Size.x, Size.y);
            boxCollider2D.offset = new Vector2((Size.x - 1f) / 2f, (Size.y - 1f) / 2f);
        }
    }
}
