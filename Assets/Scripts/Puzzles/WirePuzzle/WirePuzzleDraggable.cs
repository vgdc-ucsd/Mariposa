using UnityEngine;

public class WirePuzzleDraggable : MonoBehaviour
{
    public WirePuzzleWire Wire;
    private WirePuzzleTail connectedTail;
    public WirePuzzleTail ConnectedTail
    {
        get => connectedTail;
        set
        {
            connectedTail = value;
            if (connectedTail != null)
            {
                transform.position = connectedTail.transform.position;
                WirePuzzle.Instance.OnMoveWire();
            }
            else transform.localPosition = new Vector3(1, 0, 0);
            UpdateLineRenderer();
        }
    }
    public bool IsMatched
    { get => ConnectedTail != null && ConnectedTail.Wire == Wire; }
    
    private Vector3 mouseOffset;
    private bool isDragging = false;

    private SpriteRenderer headSprite;
    private Transform baseTransform;
    private LineRenderer lineRenderer;
    private Transform[] linePoints;

    public void InitializeWireHead()
    {
        headSprite = GetComponentInChildren<SpriteRenderer>();
        headSprite.color = Wire.Color;

        lineRenderer = transform.GetComponentInChildren<LineRenderer>();
        lineRenderer.startColor = Wire.Color;
        lineRenderer.endColor = Wire.Color;

        baseTransform = transform.parent.GetChild(0);
        baseTransform.GetComponent<SpriteRenderer>().color = Wire.Color;

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
        isDragging = true;
        mouseOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
