using UnityEngine;
using UnityEngine.UI;

public class WirePuzzleTail : WirePuzzleReceiver
{
    public void InitializeWireTail()
    {
        if (MatchingDraggable == null) Debug.LogWarning($"{transform.parent.name} Draggable MatchingDraggable not set");

        GetComponent<Image>().color = Color;
    }
}
