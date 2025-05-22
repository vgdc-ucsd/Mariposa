using UnityEngine;

public abstract class WirePuzzleReceiver : MonoBehaviour
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

    public WirePuzzleDraggable MatchingDraggable;
    public int layer;

    public bool IsMatched
    { get => ConnectedDraggable != null && ConnectedDraggable == MatchingDraggable; }
}