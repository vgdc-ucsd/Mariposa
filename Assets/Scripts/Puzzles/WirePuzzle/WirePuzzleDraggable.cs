using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WirePuzzleDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
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

    public List<WirePuzzleReceiver> ConnectedReceivers = new();
    [SerializeField] private WirePuzzleVisuals wireVisuals;
    private Vector3 origPos;

    public void InitializeWireDraggable(int index)
    {
        origPos = GetComponent<RectTransform>().localPosition;
        ConnectedTail = null;

        wireVisuals.InitializeVisuals();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            DisconnectWire();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        wireVisuals.BeginDragVisuals(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        wireVisuals.DragUpdateVisuals();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Check for wire tail
        eventData.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        foreach (var hit in raycastResults)
        {
            if (hit.gameObject.GetComponent<WirePuzzleReceiver>() != null)
            {
                TryConnectWire(hit.gameObject.GetComponent<WirePuzzleReceiver>());
                return;
            }
        }

        // If did not connect then disconnect wire
        SnapBackToPos();
    }

    private void TryConnectWire(WirePuzzleReceiver wirePuzzleReceiver)
    {
        if (wirePuzzleReceiver is WirePuzzleTail)
        {
            ConnectedTail = (WirePuzzleTail)wirePuzzleReceiver;
            // NOT NICE CODE
            AddConnectedNode(wirePuzzleReceiver);
        }
        else if (wirePuzzleReceiver is WirePuzzleNode)
        {
            AddConnectedNode(wirePuzzleReceiver);
        }
    }

    public void DisconnectWire()
    {
        ConnectedTail = null;
        ConnectedReceivers[^1].ConnectedDraggable = null;
        if (ConnectedReceivers.Count > 0) ConnectedReceivers.RemoveAt(ConnectedReceivers.Count - 1);

        // Reset position of "draggable" game object
        SnapBackToPos();
    }

    public void AddConnectedNode(WirePuzzleReceiver receiver)
    {
        if (ConnectedReceivers.Count + 1 == receiver.layer)
        {
            ConnectedReceivers.Add(receiver);
            receiver.ConnectedDraggable = this;
            // Set position of "draggable" game object
            transform.position = receiver.transform.position;
            wireVisuals.AddedNodeVisuals(receiver);
        }
        else
        {
            SnapBackToPos();
        }
    }

    public void SnapBackToPos()
    {
        if (ConnectedReceivers.Count == 0)
        {
            GetComponent<RectTransform>().localPosition = origPos;
            wireVisuals.SnapBackVisuals(this);
        }
        else
        {
            // "^1" is shorthand for last index
            transform.position = ConnectedReceivers[^1].transform.position;
            wireVisuals.SnapBackVisuals(this);
        }
    }
}
