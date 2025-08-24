using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// NOTE: This script is for the intermediate nodes in the wire puzzle. It's not named very well cause I couldn't decide what to name this class.
public class WirePuzzleNode : WirePuzzleReceiver, IPointerClickHandler
{
    public void InitializeWireNode()
    {
        if (MatchingDraggable == null) Debug.LogWarning($"{transform.parent.name} Draggable MatchingDraggable not set");

        column = transform.parent.GetSiblingIndex();
        GetComponent<Image>().color = Color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2 || eventData.button == PointerEventData.InputButton.Right)
        {
            ConnectedDraggable?.DisconnectWire(layer);
        }
    }
}
