using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockPuzzleBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Vector2Int Position;
    public Vector2Int Size;
    public Vector2Int[] Cells;
    public GameObject PreviewPrefab;
    public bool IsFixed = false;

    [SerializeField] private bool isInGrid = false;
    private Vector2Int originalPosition;
    private Vector3 stagingPosition = Vector3.one;
    private Vector2 dragOffset;

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
            BlockPuzzle.Instance.SetBlockInGrid(this, Position);
            isInGrid = true;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            Position = Vector2Int.zero;
            if (PreviewPrefab != null && preview == null)
            {
                GameObject previewObj = Instantiate(PreviewPrefab, transform.parent);
                preview = previewObj.GetComponent<BlockPreview>();
                preview.SetSprite(rectTransform.sizeDelta, image.sprite, image.color);
                preview.Hide();
            }
            
            if (stagingPosition == Vector3.one) stagingPosition = rectTransform.anchoredPosition;
        }
    }

    private void OnDestroy()
    {
        if (preview != null) Destroy(preview.gameObject);
    }

    public void SetPosition(Vector2 pos)
    {
        rectTransform.position = pos;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsFixed) return;

        Vector2 pointerPosition = eventData.position;

        Vector2 blockScreenPosition = RectTransformUtility.WorldToScreenPoint(
            eventData.pressEventCamera, 
            rectTransform.position);

        dragOffset = blockScreenPosition - pointerPosition;

        canvasGroup.alpha = 0.7f;

        canvasGroup.blocksRaycasts = false;
        
        if (isInGrid)
        {
            originalPosition = Position;
            BlockPuzzle.Instance.ClearBlockFromGrid(this);
        }

        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsFixed) return;

        Vector2 pointerPosition = eventData.position;

        Vector2 targetScreenPosition = pointerPosition + dragOffset;

        Vector3 worldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform.parent as RectTransform,
            targetScreenPosition,
            eventData.pressEventCamera,
            out worldPosition);

        rectTransform.position = worldPosition;

        if (preview != null)
        {
            if (BlockPuzzle.Instance.HoveredSlot != null)
            {
                // TODO: fix
                Vector2Int nearestGridPos = BlockPuzzle.Instance.HoveredSlot.GridPosition;

                bool isValidPosition = BlockPuzzle.Instance.IsPositionValidForBlock(nearestGridPos, this);
                Vector3 previewWorldPos = BlockPuzzle.Instance.GetSlotTransformAtPosition(nearestGridPos).anchoredPosition;
                
                if (isValidPosition) ShowPreview(previewWorldPos, true);
                else ShowPreview(previewWorldPos, false);
            }
            else
            {
                ShowPreview(stagingPosition, true);
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
            // TODO: fix
            Vector2Int targetPosition = BlockPuzzle.Instance.HoveredSlot.GridPosition;

            Debug.Log($"Valid Position: {BlockPuzzle.Instance.IsPositionValidForBlock(targetPosition, this)}");
            if (BlockPuzzle.Instance.IsPositionValidForBlock(targetPosition, this))
            {
                BlockPuzzle.Instance.SetBlockInGrid(this, targetPosition);
                isInGrid = true;
            }
            else
            {
                if (isInGrid) BlockPuzzle.Instance.SetBlockInGrid(this, originalPosition);
                else rectTransform.anchoredPosition = stagingPosition;
            }
        }
        else
        {
            rectTransform.anchoredPosition = stagingPosition;
            isInGrid = false;
        }

        if (preview != null) preview.Hide();
    }

    private void ShowPreview(Vector3 position, bool isValid)
    {
        if (preview == null) return;

        preview.SetPosition(position);

        if (isValid) preview.SetSprite(rectTransform.sizeDelta, image.sprite, image.color);
        else preview.SetSprite(rectTransform.sizeDelta, image.sprite, new Color(1f, 0.3f, 0.3f));
        
        preview.Show();
    }
}
