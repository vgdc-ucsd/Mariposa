using UnityEngine;

public class BlockPuzzleBlock : MonoBehaviour
{
    public Vector2Int size;
    public Vector2Int position;

    // Move the block in a given direction if possible
    public bool Move(Vector2Int direction)
    {
        if (BlockPuzzle.Instance.CanMove(this, direction))
        {
            // Clear the block's current position from the grid
            BlockPuzzle.Instance.ClearBlockFromGrid(this);

            // Move block to the new position
            position += direction;

            // Update block's world position (visually)
            transform.position = BlockPuzzle.Instance.GridToWorldPosition(position);

            // Update grid with new position
            BlockPuzzle.Instance.SetBlockInGrid(this, position);
            
            return true;
        }
        return false;
    }
}
