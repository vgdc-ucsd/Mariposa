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


    public void InitializeWireTail()
    {
    }

    public Vector3 GetConnectedPosition(int layer)
    {
        return new Vector3(transform.position.x, transform.position.y, (layer + 1) * -1);
    }
}
