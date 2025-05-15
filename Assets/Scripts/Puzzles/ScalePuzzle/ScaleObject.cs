using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScaleObject : MonoBehaviour
{
    public int weight;
    [HideInInspector] public bool onScale;
    private bool dragging;
    private RectTransform dragTarget;
    private RectTransform rectTransform, ghostRectTransform;
    private Rect rect;
    private Vector3 mousePos;
    private bool justClicked = false;
    private Image image, ghostImage;
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
        ghostRectTransform = ScalePuzzle.Instance.ghost.GetComponent<RectTransform>();
        ghostImage = ScalePuzzle.Instance.ghost.GetComponent<Image>();
        if (GetType() != typeof(MysteryBox)) OnClick();
        rectTransform.localScale = Vector3.one;
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
        ghostRectTransform.SetParent(transform, false);
        ghostImage.sprite = image.sprite;
        ghostRectTransform.sizeDelta = rect.size;
    }

    public void Drag()
    {
        rectTransform.position = mousePos;

        RectTransform target = null;
        foreach (RectTransform rt in ScalePuzzle.Instance.dragTargets)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rt, mousePos))
            {
                target = rt;
                if (target.TryGetComponent<ScaleHand>(out ScaleHand scaleHand))
                {
                    ScalePuzzle.Instance.ghost.SetActive(true);
                    ghostImage.enabled = true;
                    ghostRectTransform.position = transform.position;
                    scaleHand.FitToPlatform(ghostRectTransform);
                    ghostRectTransform.localScale = Vector3.one;
                }
            }
        }
        if (target == null && ScalePuzzle.Instance.ghost.activeInHierarchy)
        {
            ScalePuzzle.Instance.ghost.transform.SetParent(transform, false);
            ScalePuzzle.Instance.ghost.SetActive(false);
        } 


        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
        {
            if (target != null)
            {
                SnapToTarget(target);
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
        bool isHand = target.TryGetComponent<ScaleHand>(out ScaleHand scaleHand);

        justClicked = true;
        if (target != dragTarget || isHand) // dragging from selection area to selection area should destroy object
        {
            if (dragTarget.TryGetComponent<ScaleHand>(out ScaleHand prevScaleHand))
            {
                prevScaleHand.RemoveObject(this);
                fromHand = true;
            }
            dragTarget = target;
            if (isHand)
            {
                PlaceOnScaleHand(scaleHand);
                toHand = true;
            }
            else if (GetType() != typeof(MysteryBox)) Despawn();
            else
            {
                rectTransform.transform.position = mousePos;
                rectTransform.SetParent(target.transform, true);
            }
            if (fromHand || toHand) ScalePuzzle.Instance.MoveHands();

            dragging = false;
            ScalePuzzle.Instance.isDragging = false;
            ghostImage.enabled = false;

        }

    }
    public void PlaceOnScaleHand(ScaleHand scaleHand)
    {
        scaleHand.AddObject(this);
    }

    private void Despawn()
    {
        ghostRectTransform.SetParent(ScalePuzzle.Instance.transform, true);
        ghostImage.enabled = false;
        Destroy(gameObject);
    }
}
