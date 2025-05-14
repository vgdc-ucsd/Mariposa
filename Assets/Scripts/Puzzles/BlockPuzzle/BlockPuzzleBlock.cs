using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockPuzzleBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Vector2Int GridPos;
    public Vector2Int Size;
    public Vector2Int[] Cells;
    public GameObject PreviewPrefab;
    public bool IsFixed = false;

    [SerializeField] private bool isInGrid = false;
    private Vector2Int lastPos;
    private Vector3 stagingPos = Vector3.one;
    private Vector2 dragOffset;
    private Vector2Int dragOffsetCell;

    private RectTransform rectTransform;
    private Image image;
    private BlockPreview preview = null;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    public void InitializeBlock()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        if (IsFixed)
        {
            BlockPuzzle.Instance.SetBlockInGrid(this, GridPos);
            isInGrid = true;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            GridPos = Vector2Int.zero;
            if (PreviewPrefab != null && preview == null)
            {
                GameObject previewObj = Instantiate(PreviewPrefab, transform.parent);
                preview = previewObj.GetComponent<BlockPreview>();
                preview.SetSprite(rectTransform.sizeDelta, image.sprite, image.color);
                preview.Hide();
            }
            
            if (stagingPos == Vector3.one) stagingPos = rectTransform.anchoredPosition;
        }
    }

    private void OnDestroy()
    {
        if (preview != null) Destroy(preview.gameObject);
    }

    public void SetPosition(Vector2 worldPos)
    {
        rectTransform.position = worldPos;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsFixed) return;

        Vector2 pointerPos = eventData.position;
        Vector2 blockScreenPos = RectTransformUtility.WorldToScreenPoint(
            eventData.pressEventCamera, 
            rectTransform.position);

        // TODO: get proper cell offset
        dragOffset = blockScreenPos - pointerPos;
        dragOffsetCell = new Vector2Int(
            // (int) Mathf.Floor(dragOffset.x / (2 * BlockPuzzle.Instance.SlotContainer.cellSize.x)),
            // (int) Mathf.Floor(dragOffset.y / (2 * BlockPuzzle.Instance.SlotContainer.cellSize.y))
            (int) Mathf.Floor((rectTransform.position.x - pointerPos.x) / BlockPuzzle.Instance.SlotContainer.cellSize.x),
            (int) Mathf.Floor((rectTransform.position.y - pointerPos.y) / BlockPuzzle.Instance.SlotContainer.cellSize.y)
        );
        // Debug.Log($"Block position: {rectTransform.position}    Pointer position: {pointerPos}    Difference: {dragOffsetCell}");

        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;
        
        if (isInGrid)
        {
            lastPos = GridPos;
            BlockPuzzle.Instance.ClearBlockFromGrid(this);
        }

        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsFixed) return;

        Vector2 pointerPos = eventData.position;
        Vector2 targetScreenPos = pointerPos + dragOffset;

        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform.parent as RectTransform,
            targetScreenPos,
            eventData.pressEventCamera,
            out worldPos);

        rectTransform.position = worldPos;

        if (preview != null)
        {
            if (BlockPuzzle.Instance.HoveredSlot != null)
            {
                // TODO: fix, depends on cell offset
                // Vector2Int nearestGridPos = BlockPuzzle.Instance.HoveredSlot.GridPos - dragOffsetCell;
                Vector2Int targetGridPos = BlockPuzzle.Instance.HoveredSlot.GridPos;

                if (BlockPuzzle.Instance.IsPositionValidForBlock(targetGridPos, this))
                {
                    Vector3 previewWorldPos = BlockPuzzle.Instance.GetSlotTransformAtPosition(targetGridPos).position;
                    ShowPreview(previewWorldPos, true);
                }
                else
                {
                    Vector3 previewWorldPos = BlockPuzzle.Instance.GetSlotTransformAtPosition(lastPos).position;
                    ShowPreview(previewWorldPos, false);
                }
            }
            else
            {
                ShowPreview(stagingPos, true);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsFixed) return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (BlockPuzzle.Instance.HoveredSlot != null)
        {
            // TODO: fix, depends on cell offset
            // Vector2Int targetGridPos = BlockPuzzle.Instance.HoveredSlot.GridPos - dragOffsetCell;
            Vector2Int targetGridPos = BlockPuzzle.Instance.HoveredSlot.GridPos;

            Debug.Log($"Placing block in slot={targetGridPos}, valid={BlockPuzzle.Instance.IsPositionValidForBlock(targetGridPos, this)}");
            if (BlockPuzzle.Instance.IsPositionValidForBlock(targetGridPos, this))
            {
                BlockPuzzle.Instance.SetBlockInGrid(this, targetGridPos);
                isInGrid = true;
            }
            else
            {
                if (isInGrid) BlockPuzzle.Instance.SetBlockInGrid(this, lastPos);
                else rectTransform.anchoredPosition = stagingPos;
            }
        }
        else
        {
            Debug.Log("Hovered Slot = null");
            rectTransform.anchoredPosition = stagingPos;
            isInGrid = false;
        }

        if (preview != null) preview.Hide();
    }

    private void ShowPreview(Vector3 worldPos, bool isValid)
    {
        if (preview == null) return;

        preview.SetPosition(worldPos);

        if (isValid) preview.SetSprite(rectTransform.sizeDelta, image.sprite, image.color);
        else preview.SetSprite(rectTransform.sizeDelta, image.sprite, new Color(1f, 0.3f, 0.3f));
        
        preview.Show();
    }
}
