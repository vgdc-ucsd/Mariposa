using UnityEngine;

public class BlockPuzzleBlock : MonoBehaviour
{
    public Vector2Int size;
    public Vector2Int position;

    private Vector3 mouseOffset;
    private bool isDragging = false;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;

    public void InitializeBlock()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        if (spriteRenderer != null && boxCollider2D != null) AdjustVisualSize();
        else Debug.LogWarning("SpriteRenderer not found on child object or BoxCollider2D not found on object.");
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
            mousePosition.z = 0;  // Ensure the block stays on the 2D plane
            transform.position = mousePosition;
        }
    }

    private void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;

            // Snap the block to the nearest grid position (limited to one cell)
            Vector2Int snappedPosition = SnapToGrid(transform.position);

            // Check if the move is valid in each direction (up, down, left, right)
            Vector2Int direction = snappedPosition - position;

            if (direction.magnitude == 1 && BlockPuzzle.Instance.CanMove(this, direction))
            {
                // Use MoveBlock to move the block in the grid
                BlockPuzzle.Instance.MoveBlock(this, direction);
            }
            else
            {
                Debug.Log($"Failed to move {this}");
                // Return to original position if move is invalid
                transform.position = BlockPuzzle.Instance.GridToWorldPosition(position);
            }
        }
    }

    // Snap block's world position to the nearest grid position
    private Vector2Int SnapToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x);
        int y = Mathf.RoundToInt(worldPosition.y);
        return new Vector2Int(x, y);
    }

    // Adjust the visual size of the block based on its size
    private void AdjustVisualSize()
    {
        if (spriteRenderer != null && boxCollider2D != null)
        {
            spriteRenderer.transform.localScale = new Vector3(size.x, size.y, 1);
            Vector3 adjustedPosition = new Vector3((size.x - 1f) / 2f, (size.y - 1f) / 2f, 0);
            spriteRenderer.transform.localPosition = adjustedPosition;
            
            boxCollider2D.size = new Vector2(size.x, size.y);
            boxCollider2D.offset = new Vector2((size.x - 1f) / 2f, (size.y - 1f) / 2f);
        }
    }
}
