using System.Net.Sockets;
using UnityEngine;

public class ScaleObject : MonoBehaviour
{
    public float weight;
    [HideInInspector] public bool onScale;
    private bool dragging;
    private RectTransform dragTarget;
    private RectTransform rectTransform;
    private Rect rect;
    private Vector3 mousePos;
    private bool justClicked = false;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rect = rectTransform.rect;
    }

    private void Start()
    {
        dragTarget = GetComponentInParent<RectTransform>();
    }

    private void Update()
    {
        mousePos = RectTransformUtility.WorldToScreenPoint(Camera.main, Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (dragging) Drag();
        if (Input.GetMouseButtonDown(0))
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePos))
            {
                Debug.Log("clicked");
                OnClick();
            }
        }
        else if (!dragging) rectTransform.localPosition = Vector3.zero;

        justClicked = false;
    }

    public void OnClick()
    {
        if (dragging || justClicked) return;
        dragging = true;
        justClicked = true;
    }

    public void Drag()
    {
        rectTransform.position = mousePos;


        if (Input.GetMouseButtonDown(0))
        {
            foreach (RectTransform target in ScalePuzzle.Instance.dragTargets)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(target, mousePos))
                {
                    SnapToTarget(target);
                    return;
                }
            }
            SnapToTarget(dragTarget);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            foreach (RectTransform target in ScalePuzzle.Instance.dragTargets)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(target, mousePos))
                {
                    bool sameTarget = (target == dragTarget);
                    if (!sameTarget) SnapToTarget(target);
                    
                    return;
                }
            }
        }
    }

    private void SnapToTarget(RectTransform target)
    {
        Debug.Log("placed");
        bool fromHand = false;
        bool toHand = false;
        if (dragTarget.TryGetComponent<ScaleHand>(out ScaleHand prevScaleHand))
        {
            prevScaleHand.RemoveObject(this);
            fromHand = true;
        }

        if (target.TryGetComponent<ScaleHand>(out ScaleHand scaleHand))
        {
            PlaceOnScaleHand(scaleHand);
            toHand = true;
        }
        if (fromHand || toHand) ScalePuzzle.Instance.MoveHands();

        justClicked = true;
        dragTarget = target;
        rectTransform.SetParent(target.transform, false);
        rectTransform.localPosition = Vector3.zero;
        dragging = false;
    }
    public void PlaceOnScaleHand(ScaleHand scaleHand)
    {
        scaleHand.AddObject(this);
    }

    
}
