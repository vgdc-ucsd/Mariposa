using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockPuzzleBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Vector2Int GridPos;
    public Vector2Int Size;
    public Vector2Int[] Cells;
    public GameObject PreviewPrefab;

    [SerializeField] private bool isFixed = false;
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
        image.alphaHitTestMinimumThreshold = 0.95f;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        if (isFixed)
        {
            BlockPuzzle.Instance.SetBlockInGrid(this, GridPos);
            isInGrid = true;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            GridPos = -1 * Vector2Int.one;
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
        if (isFixed) return;

        Vector2 pointerPos = eventData.position;

        Vector3 pointerWorldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform.parent as RectTransform,
            pointerPos,
            eventData.pressEventCamera,
            out pointerWorldPos);

        dragOffsetCell = CalculateCellOffset(pointerWorldPos, rectTransform.position);

        Vector2 blockScreenPos = RectTransformUtility.WorldToScreenPoint(
            eventData.pressEventCamera, 
            rectTransform.position);
        dragOffset = blockScreenPos - pointerPos;

        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;

        if (isInGrid)
        {
            lastPos = GridPos;
        }
        BlockPuzzle.Instance.ClearBlockFromGrid(this);

        transform.SetAsLastSibling();
    }

    // TODO: fix cell offset misalignment
    private Vector2Int CalculateCellOffset(Vector3 pointerWorldPos, Vector3 transformWorldPos)
    {
        Vector2Int output = new Vector2Int(
            Mathf.FloorToInt(
                (pointerWorldPos.x - transformWorldPos.x + (Size.x - 1) * BlockPuzzle.Instance.CellDiameter) / 
                (2 * BlockPuzzle.Instance.CellDiameter)),
            Mathf.FloorToInt(
                (pointerWorldPos.y - transformWorldPos.y + (Size.y - 1) * BlockPuzzle.Instance.CellDiameter) 
                / (2 * BlockPuzzle.Instance.CellDiameter))
        );
        if (output.x < 0) output.x = 0;
        if (output.x >= Size.x) output.x = Size.x - 1;
        if (output.y < 0) output.y = 0;
        if (output.y >= Size.y) output.y = Size.y - 1;
        Debug.Log($"Offset: {output}");
        return output;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isFixed) return;

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
                Vector2Int targetGridPos = BlockPuzzle.Instance.HoveredSlot.GridPos - dragOffsetCell;
                BlockPuzzle.Instance.DebugSelectSlot(targetGridPos);
                Vector3 previewWorldPos = BlockPuzzle.Instance.GetSlotTransformAtPosition(targetGridPos).anchoredPosition;
                previewWorldPos += new Vector3(
                    (-3.5f - 1f + Size.x) * BlockPuzzle.Instance.CellDiameter - (Size.x - 1f) * BlockPuzzle.Instance.CellDiameter / 2f,
                    (3.5f - 1f + Size.y) * BlockPuzzle.Instance.CellDiameter - (Size.y - 1f) * BlockPuzzle.Instance.CellDiameter / 2f,
                    0f
                );
                ShowPreview(previewWorldPos, BlockPuzzle.Instance.IsPositionValidForBlock(targetGridPos, this));
            }
            else
            {
                ShowPreview(stagingPos, true);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isFixed) return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (BlockPuzzle.Instance.HoveredSlot != null)
        {
            Vector2Int targetGridPos = BlockPuzzle.Instance.HoveredSlot.GridPos - dragOffsetCell;

            if (BlockPuzzle.Instance.IsPositionValidForBlock(targetGridPos, this))
            {
                BlockPuzzle.Instance.SetBlockInGrid(this, targetGridPos);
                isInGrid = true;
            }
            else
            {
                if (isInGrid)
                {
                    BlockPuzzle.Instance.SetBlockInGrid(this, lastPos);
                }
                else
                {
                    rectTransform.anchoredPosition = stagingPos;
                    GridPos = -1 * Vector2Int.one;
                }
            }
        }
        else
        {
            rectTransform.anchoredPosition = stagingPos;
            GridPos = -1 * Vector2Int.one;
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
