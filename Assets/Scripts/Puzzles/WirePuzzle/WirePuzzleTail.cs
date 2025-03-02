using UnityEngine;

public class WirePuzzleTail : MonoBehaviour
{
    private WirePuzzleDraggable connectedDraggable;
    public WirePuzzleDraggable ConnectedDraggable
    {
        get => connectedDraggable;
        set
        {
            if (value != connectedDraggable)
            {
                if (connectedDraggable != null && value != null)
                {
                    var oldDraggable = connectedDraggable;
                    connectedDraggable = null;
                    oldDraggable.ConnectedTail = null;
                }
                connectedDraggable = value;
            }
        }
    }

    private BoxCollider2D boxCollider2D;

    public void InitializeWireTail()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    public Vector3 GetConnectedPosition(int layer)
    {
        return new Vector3(transform.position.x, transform.position.y, (layer + 1) * -1);
    }
}
