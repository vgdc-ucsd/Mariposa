using UnityEngine;

public class BlockPuzzleBlock : MonoBehaviour
{
    public Vector2Int size;  // Size of the block (width, height)
    public Vector2Int position;  // Position of the block on the grid

    private Vector3 mouseOffset;  // Offset between the mouse position and block's position
    private bool isDragging = false;  // Is the block being dragged?

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
            // Snap the block to the nearest grid position
            Vector2Int snappedPosition = SnapToGrid(transform.position);

            // Check if the move is valid
            if (BlockPuzzle.Instance.CanMove(this, snappedPosition - position))
            {
                position = snappedPosition;
                transform.position = BlockPuzzle.Instance.GridToWorldPosition(position);
                BlockPuzzle.Instance.SetBlockInGrid(this, position);  // Update the grid with new position
            }
            else
            {
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
}
