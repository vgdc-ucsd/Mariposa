using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WirePuzzleVisuals : MonoBehaviour
{
    [Header("Wire Segment Settings")]
    public Color Color;
    [SerializeField] float wireThickness;
    [SerializeField] Vector2 segmentInitialOffset = Vector2.zero;
    private float canvasScaleFactor = 1.0f;

    [Header("References")]
    [SerializeField] GameObject WireSegmentPrefab;
    public List<GameObject> WireSegments = new();

    private bool hasAwoken = false; // This is only for OnRectTransformDimensionsChange() function is called before Awake()
    private void Awake()
    {
        canvasScaleFactor = GetComponentInParent<Canvas>().scaleFactor;
        hasAwoken = true;
    }

    private void OnRectTransformDimensionsChange()
    {
        if (!hasAwoken) return;
        Canvas canvas = GetComponentInParent<Canvas>(true);
        if (canvas != null) canvasScaleFactor = canvas.scaleFactor;
    }

    public void InitializeVisuals()
    {
        // Instantiate prefab
        RectTransform thisRectTransform = GetComponent<RectTransform>();
        GameObject newSegment = Instantiate(WireSegmentPrefab, transform);
        RectTransform newSegmentTransform = newSegment.GetComponent<RectTransform>();

        // Set Size
        newSegmentTransform.sizeDelta = thisRectTransform.sizeDelta;
        // Set Color
        newSegment.GetComponent<Image>().color = Color;
        // Set Starting Position
        Vector2 pivotOffset = newSegmentTransform.sizeDelta.y / 2.0f * Vector2.down; // segments are pivoted at (0.5, 1.0)
        newSegmentTransform.localPosition = thisRectTransform.anchoredPosition - pivotOffset;
        WireSegments.Add(newSegment);
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
            instantiated.GetComponent<RectTransform>().localPosition = segmentInitialOffset;
        }
    }

    public void DragUpdateVisuals()
    {
        RectTransform wireSegment = WireSegments[^1].GetComponent<RectTransform>();
        Vector2 mousePos = Input.mousePosition;

        // Set length (By changing height of rect transform)
        wireSegment.sizeDelta = new(wireThickness, Vector2.Distance(mousePos, wireSegment.position) / canvasScaleFactor);

        // Add rotation
        float rotation = Mathf.Atan2(wireSegment.position.y - mousePos.y, wireSegment.position.x - mousePos.x) * Mathf.Rad2Deg - 90f;
        wireSegment.localEulerAngles = new(0, 0, rotation);
    }

    public void AddedNodeVisuals(WirePuzzleReceiver receiver)
    {
        RectTransform wireSegment = WireSegments[^1].GetComponent<RectTransform>();

        // Set size & rotation of visuals
        wireSegment.sizeDelta = new(wireThickness, Vector2.Distance(receiver.transform.position, wireSegment.position) / canvasScaleFactor);

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

                wireSegment.sizeDelta = new(wireThickness, Vector2.Distance(receiver.transform.position, wireSegment.position));

                float rotation = Mathf.Atan2(wireSegment.position.y - receiver.transform.position.y,
                                    wireSegment.position.x - receiver.transform.position.x) * Mathf.Rad2Deg - 90f;
                wireSegment.localEulerAngles = new(0, 0, rotation);
            }
        }
    }
}
