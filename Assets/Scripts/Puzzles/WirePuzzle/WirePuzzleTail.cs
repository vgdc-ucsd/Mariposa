using UnityEngine;

public class WirePuzzleTail : WirePuzzleReceiver
{
    public void InitializeWireTail()
    {
        if (MatchingDraggable == null) Debug.LogWarning($"{transform.parent.name} Draggable MatchingDraggable not set");
    }
}
