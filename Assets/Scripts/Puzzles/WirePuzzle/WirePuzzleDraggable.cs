using UnityEngine;

public class WirePuzzleDraggable : MonoBehaviour
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
                transform.position = connectedTail.GetConnectedPosition(index);
                WirePuzzle.Instance.OnMoveWire();
            }
            else 
            {
                transform.localPosition = defaultPosition;
            }
            
            UpdateLineRenderer();
        }
    }
    public bool IsMatched
    { get => ConnectedTail != null && ConnectedTail == MatchingTail; }
    
    private Vector3 mouseOffset;
    private bool isDragging = false;

    private SpriteRenderer headSprite;
    private Transform baseTransform;
    private LineRenderer lineRenderer;
    private Transform[] linePoints;
    private Vector3 defaultPosition;
    private int index;

    public void InitializeWireDraggable(int index)
    {
        if (MatchingTail == null) Debug.LogWarning($"{transform.parent.name} Draggable MatchingTail not set");

        defaultPosition = transform.localPosition;
        this.index = index;

        headSprite = GetComponentInChildren<SpriteRenderer>();
        headSprite.sortingOrder = index + 1;

        lineRenderer = transform.GetComponentInChildren<LineRenderer>();
        lineRenderer.startColor = Color;
        lineRenderer.endColor = Color;
        lineRenderer.sortingOrder = index + 1;

        baseTransform = transform.parent.GetChild(0);
        baseTransform.GetComponent<SpriteRenderer>().color = Color;

        SetupLineRenderer(new Transform[]{baseTransform, transform});

        ConnectedTail = null;
    }

    private void SetupLineRenderer(Transform[] points)
    {
        lineRenderer.positionCount = points.Length;
        linePoints = points;
        UpdateLineRenderer();
    }

    private void OnMouseDown()
    {
        if (!WirePuzzle.Instance.IsComplete)
        {
            isDragging = true;
            mouseOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + mouseOffset;
            mousePosition.z = 0;
            transform.position = mousePosition;
            UpdateLineRenderer();
        }
    }

    private void UpdateLineRenderer()
    {
        lineRenderer.SetPosition(0, linePoints[0].position);
        lineRenderer.SetPosition(1, linePoints[1].position);
    }

    private void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;

            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + mouseOffset;
            bool placedWire = false;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPos, 0.01f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.TryGetComponent<WirePuzzleTail>(out var tail))
                {
                    ConnectedTail = tail;
                    placedWire = true;
                    break;
                }
            }

            if (!placedWire) ConnectedTail = null;
        }
    }
}
