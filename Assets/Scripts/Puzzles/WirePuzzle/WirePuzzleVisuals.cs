using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WirePuzzleVisuals : MonoBehaviour
{
    [Header("Wire Segment Settings")]
    public Color Color;
    [SerializeField] Vector2 origSize;
    [SerializeField] Vector2 origPos;

    [Header("References")]
    [SerializeField] GameObject WireSegmentPrefab;
    public List<GameObject> WireSegments = new();

    public void InitializeVisuals()
    {
        // Instantiate prefab
        GameObject instantiated = Instantiate(WireSegmentPrefab, transform);
        instantiated.GetComponent<RectTransform>().sizeDelta = origSize;
        WireSegments.Add(instantiated);

        // Set Color
        instantiated.GetComponent<Image>().color = Color;

        // Set Starting Position
        instantiated.GetComponent<RectTransform>().localPosition = origPos;
    }

    public void BeginDragVisuals(WirePuzzleDraggable draggable)
    {
        if (draggable.ConnectedReceivers.Count > 0)
        {
            // Should not be able to drag wire from the tail
            if (draggable.ConnectedReceivers[^1] is WirePuzzleTail) return;

            // Instantiate a new wire segment
            GameObject instantiated = Instantiate(WireSegmentPrefab, transform);
            WireSegments.Add(instantiated);
            instantiated.GetComponent<Image>().color = Color;
            instantiated.transform.position = draggable.ConnectedReceivers[^1].transform.position;
        }
        else
        {
            // Instantiate a new wire segment
            GameObject instantiated = Instantiate(WireSegmentPrefab, transform);
            WireSegments.Add(instantiated);
            instantiated.GetComponent<Image>().color = Color;
            instantiated.GetComponent<RectTransform>().localPosition = new(0, -50f);
        }
    }

    public void DragUpdateVisuals()
    {
        RectTransform wireSegment = WireSegments[^1].GetComponent<RectTransform>();
        Vector2 mousePos = Input.mousePosition;

        // Set length (By changing height of rect transform)
        wireSegment.sizeDelta = new(origSize.x, Vector2.Distance(mousePos, wireSegment.position));

        // Add rotation
        float rotation = Mathf.Atan2(wireSegment.position.y - mousePos.y, wireSegment.position.x - mousePos.x) * Mathf.Rad2Deg - 90f;
        wireSegment.localEulerAngles = new(0, 0, rotation);
    }

    public void AddedNodeVisuals(WirePuzzleReceiver receiver)
    {
        RectTransform wireSegment = WireSegments[^1].GetComponent<RectTransform>();

        // Set size & rotation of visuals
        wireSegment.sizeDelta = new(origSize.x, Vector2.Distance(receiver.transform.position, wireSegment.position));

        float rotation = Mathf.Atan2(wireSegment.position.y - receiver.transform.position.y,
                            wireSegment.position.x - receiver.transform.position.x) * Mathf.Rad2Deg - 90f;
        wireSegment.localEulerAngles = new(0, 0, rotation);
    }

    public void SnapBackVisuals(WirePuzzleDraggable draggable)
    {
        RectTransform wireSegment = WireSegments[^1].GetComponent<RectTransform>();
        
        if (WireSegments.Count > 1)
        {
            if (WireSegments.Count > draggable.ConnectedReceivers.Count)
            {
                WireSegments.Remove(wireSegment.gameObject);
                Destroy(wireSegment.gameObject);
            }
            else
            {
                WirePuzzleReceiver receiver = draggable.ConnectedReceivers[^1];

                wireSegment.sizeDelta = new(origSize.x, Vector2.Distance(receiver.transform.position, wireSegment.position));

                float rotation = Mathf.Atan2(wireSegment.position.y - receiver.transform.position.y,
                                    wireSegment.position.x - receiver.transform.position.x) * Mathf.Rad2Deg - 90f;
                wireSegment.localEulerAngles = new(0, 0, rotation);
            }
        }
    }
}
