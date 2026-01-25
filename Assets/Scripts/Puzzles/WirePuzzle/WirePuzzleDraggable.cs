using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using FMOD.Studio;
using FMODUnity;

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
    [SerializeField] private int maxHorizontalMove = 1;
    private Vector3 origPos;

    public void InitializeWireDraggable(int index)
    {
        origPos = GetComponent<RectTransform>().localPosition;
        ConnectedTail = null;

        wireVisuals.InitializeVisuals();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2 || eventData.button == PointerEventData.InputButton.Right)
        {
            DisconnectWire(Mathf.Max(ConnectedReceivers.Count - 1, 0));
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        wireVisuals.BeginDragVisuals(this);
        RuntimeManager.PlayOneShot(AudioEvents.SFX.wire_pull);
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
            if (hit.gameObject.GetComponent<WirePuzzleReceiver>() == null) continue;

            WirePuzzleReceiver receiver = hit.gameObject.GetComponent<WirePuzzleReceiver>();

            // Prevent the player from making a connection that isn't straight down or one column over
            int nodeDistance = ConnectedReceivers.Count == 0
                ? math.abs(transform.parent.GetSiblingIndex() - receiver.column)
                : math.abs(ConnectedReceivers[^1].column - receiver.column);
            if (nodeDistance > maxHorizontalMove) continue;

            // Prevent the player from making a connection to a node that is a set color
            if (receiver.onlyAllowsValidConnections && receiver.MatchingDraggable != this) continue;

            TryConnectWire(receiver);
            return;
        }

        // If did not connect then disconnect wire
        SnapBackToPos();
    }

    private void TryConnectWire(WirePuzzleReceiver wirePuzzleReceiver)
    {
        if (wirePuzzleReceiver.ConnectedDraggable != null)
        {
            wireVisuals.SnapBackVisuals(this);
            return;
        }

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

        RuntimeManager.PlayOneShot(AudioEvents.SFX.wire_connect);
    }

    public void DisconnectWire(int layer)
    {
        while (ConnectedReceivers.Count > layer)
        {
            ConnectedTail = null;
            if (ConnectedReceivers.Count > 0)
            {
                ConnectedReceivers[^1].ConnectedDraggable = null;
                ConnectedReceivers[^1].GetComponent<Image>().raycastTarget = true;
                ConnectedReceivers.RemoveAt(ConnectedReceivers.Count - 1);
            }
            // Reset position of "draggable" game object
            SnapBackToPos();
        }
        
        Image bottomReceiver = null;
        if (ConnectedReceivers.Count > 0) bottomReceiver = ConnectedReceivers[^1].GetComponent<Image>();
        if (bottomReceiver != null) bottomReceiver.raycastTarget = false;

        RuntimeManager.PlayOneShot(AudioEvents.SFX.wire_disconnect);
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

            // Turn off raycast target for connected node
            receiver.GetComponent<Image>().raycastTarget = false;
            
            // Turn on raycast target for parent node
            if (receiver.layer > 1) ConnectedReceivers[receiver.layer - 2].GetComponent<Image>().raycastTarget = true;
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
