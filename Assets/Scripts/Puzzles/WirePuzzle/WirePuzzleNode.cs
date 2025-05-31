using UnityEngine;
using UnityEngine.UI;

// NOTE: This script is for the intermediate nodes in the wire puzzle. It's not named very well cause I couldn't decide what to name this class.
public class WirePuzzleNode : WirePuzzleReceiver
{
    public Color Color;

    public void InitializeWireNode()
    {
        if (MatchingDraggable == null) Debug.LogWarning($"{transform.parent.name} Draggable MatchingDraggable not set");

        GetComponent<Image>().color = Color;
    }
}
