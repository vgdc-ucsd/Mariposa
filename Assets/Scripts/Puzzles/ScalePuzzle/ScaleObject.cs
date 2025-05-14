using UnityEngine;
using UnityEngine.UI;

public class ScaleObject : MonoBehaviour
{
    public int weight;
    [HideInInspector] public bool onScale;
    private bool dragging;
    private RectTransform dragTarget;
    private RectTransform rectTransform;
    private Rect rect;
    private Vector3 mousePos;
    private bool justClicked = false;
    private Image image;
    [HideInInspector] public bool partOfSolution;
    private Vector3 oldPos;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rect = rectTransform.rect;
        image = GetComponent<Image>();
    }

    private void Start()
    {
        dragTarget = ScalePuzzle.Instance.selectionArea.GetComponent<RectTransform>();
    }

    private void Update()
    {
        mousePos = RectTransformUtility.WorldToScreenPoint(Camera.main, Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (dragging) Drag();
        if (Input.GetMouseButtonDown(0))
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePos))
            {
                OnClick();
            }
        }

        justClicked = false;
    }

    public void OnClick()
    {
        if (dragging || justClicked || ScalePuzzle.Instance.isDragging) return;
        ScalePuzzle.Instance.isDragging = true;
        oldPos = rectTransform.position;
        dragging = true;
        justClicked = true;
        image.color = new Color(1, 1, 1, 0.5f);
    }

    public void Drag()
    {
        rectTransform.position = mousePos;

        RectTransform target = null;
        foreach (RectTransform rt in ScalePuzzle.Instance.dragTargets)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(target, mousePos))
            {
                target = rt;
            }
        }



        if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
            if (target != null)
            {
                bool sameTarget = (target == dragTarget);
                if (!sameTarget) SnapToTarget(target);
            }
            else
            {
                // if we drag onto nothing, put it back into old position
                SnapToTarget(dragTarget);
                rectTransform.position = oldPos;
            }
        }
            
    }

        

    private void SnapToTarget(RectTransform target)
    {
        bool fromHand = false;
        bool toHand = false;
        

        justClicked = true;
        if (target != dragTarget)
        {
            if (dragTarget.TryGetComponent<ScaleHand>(out ScaleHand prevScaleHand))
            {
                prevScaleHand.RemoveObject(this);
                fromHand = true;
            }
            dragTarget = target;
            rectTransform.SetParent(target.transform, true);
            if (target.TryGetComponent<ScaleHand>(out ScaleHand scaleHand))
            {
                PlaceOnScaleHand(scaleHand);
                toHand = true;
            }
            else if (GetType() != typeof(MysteryBox)) Destroy(gameObject);
            else rectTransform.transform.position = mousePos;
            if (fromHand || toHand) ScalePuzzle.Instance.MoveHands();
        }

        dragging = false;
        ScalePuzzle.Instance.isDragging = false;
        image.color = Color.white;
    }
    public void PlaceOnScaleHand(ScaleHand scaleHand)
    {
        scaleHand.AddObject(this);
    }

    
}
