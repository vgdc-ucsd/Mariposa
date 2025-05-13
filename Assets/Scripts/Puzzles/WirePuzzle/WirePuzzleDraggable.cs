using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WirePuzzleDraggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Color Color;
    public WirePuzzleTail MatchingTail;

    private WirePuzzleTail connectedTail;
    public WirePuzzleTail ConnectedTail
    {
        get => connectedTail;
        set
        {
            if (connectedTail != null && value != null)
            {
                var oldTail = connectedTail;
                connectedTail = null;
                oldTail.ConnectedDraggable = null;
            }
            else if (value == null && connectedTail != null)
            {
                var oldTail = connectedTail;
                connectedTail = null;
                oldTail.ConnectedDraggable = null;
            }

            connectedTail = value;

            if (connectedTail != null)
            {
                connectedTail.ConnectedDraggable = this;
                // transform.position = connectedTail.GetConnectedPosition(index);
                // This line checks for the solution
                WirePuzzle.Instance.OnMoveWire();
            }
        }
    }
    public bool IsMatched
    { get => ConnectedTail != null && ConnectedTail == MatchingTail; }

    // private int index;
    [SerializeField] private Image wireVisuals;
    private Coroutine currentCoroutine;
    private Vector3 visualsOrigSize;
    private Vector3 origPos;

    public void InitializeWireDraggable(int index)
    {
        if (MatchingTail == null) Debug.LogWarning($"{transform.parent.name} Draggable MatchingTail not set");

        // this.index = index;

        wireVisuals.color = Color;
        visualsOrigSize = wireVisuals.rectTransform.sizeDelta;
        origPos = transform.position;

        ConnectedTail = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        currentCoroutine = StartCoroutine(DragUpdate());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopCoroutine(currentCoroutine);

        // Check for wire tail
        eventData.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        foreach (var hit in raycastResults)
        {
            if (hit.gameObject.GetComponent<WirePuzzleTail>() != null)
            {
                TryConnectWire(hit.gameObject.GetComponent<WirePuzzleTail>());
                return;
            }
        }

        // If did not connect then disconnect wire
        DisconnectWire();
    }

    // The loop in this method is called every frame when mouse is down
    private IEnumerator DragUpdate()
    {
        RectTransform wireRectTransform = wireVisuals.rectTransform;
        while (true)
        {
            // Set length (By changing height of rect transform)
            Vector2 mousePos = Input.mousePosition;
            wireRectTransform.sizeDelta = new(visualsOrigSize.x, Vector2.Distance(mousePos, wireRectTransform.position));

            // Add rotation
            float rotation = Mathf.Atan2(wireRectTransform.position.y - mousePos.y, wireRectTransform.position.x - mousePos.x) * Mathf.Rad2Deg - 90f;
            wireRectTransform.localEulerAngles = new(0, 0, rotation);
            yield return null;
        }
    }

    private void TryConnectWire(WirePuzzleTail wirePuzzleTail)
    {
        RectTransform wireRectTransform = wireVisuals.rectTransform;

        ConnectedTail = wirePuzzleTail;

        // Set size & rotation of visuals
        wireRectTransform.sizeDelta = new(visualsOrigSize.x, Vector2.Distance(wirePuzzleTail.transform.position, wireRectTransform.position));
        float rotation = Mathf.Atan2(wireRectTransform.position.y - wirePuzzleTail.transform.position.y,
                            wireRectTransform.position.x - wirePuzzleTail.transform.position.x) * Mathf.Rad2Deg - 90f;
        wireRectTransform.localEulerAngles = new(0, 0, rotation);

        // Set position of "draggable" game object
        transform.position = wirePuzzleTail.transform.position;
    }

    public void DisconnectWire()
    {
        ConnectedTail = null;

        // Reset visuals
        wireVisuals.rectTransform.sizeDelta = visualsOrigSize;
        wireVisuals.rectTransform.localEulerAngles = new(0, 0, 0);

        // Reset position of "draggable" game object
        transform.position = origPos;
    }
}
