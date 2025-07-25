using UnityEngine;
using UnityEngine.UI;

public class WirePuzzleTail : WirePuzzleReceiver
{
    public void InitializeWireTail()
    {
        if (MatchingDraggable == null) Debug.LogWarning($"{transform.parent.name} Draggable MatchingDraggable not set");

        column = transform.parent.GetSiblingIndex();
        GetComponent<Image>().color = Color;
    }
}
