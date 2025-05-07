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
    private Vector3 stagingPosition;
    private Vector2 dragOffset;

    private RectTransform rectTransform;
    private Image image;
    private BlockPreview preview;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    public void InitializeBlock()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        canvas = GetComponentInParent<Canvas>();

        // Set up initial position and preview
        if (IsFixed)
        {
            BlockPuzzle.Instance.SetBlockInGrid(this, Position);
            isInGrid = true;
            
            // Position the UI element correctly
            rectTransform.anchoredPosition = BlockPuzzle.Instance.GridToWorldPosition(Position);
            
            // Disable dragging for fixed blocks
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            // Create preview for non-fixed blocks
            if (PreviewPrefab != null)
            {
                GameObject previewObj = Instantiate(PreviewPrefab, transform.parent);
                preview = previewObj.GetComponent<BlockPreview>();
                preview.SetSprite(rectTransform.sizeDelta, image.sprite, image.color);
                preview.Hide();
            }
            
            stagingPosition = rectTransform.anchoredPosition;
        }
    }

    private void OnDestroy()
    {
        if (preview != null)
        {
            Destroy(preview.gameObject);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsFixed) return;
        
        if (!isInGrid || (isInGrid && BlockPuzzle.Instance.CanMove(this, Vector2Int.zero)))
        {
            // Save the initial pointer position in screen space
            Vector2 pointerPosition = eventData.position;
            
            // Get the block's position in screen space
            Vector2 blockScreenPosition = RectTransformUtility.WorldToScreenPoint(
                eventData.pressEventCamera, 
                rectTransform.position);
            
            // Calculate the offset in screen space
            dragOffset = blockScreenPosition - pointerPosition;
            
            // Make the block semi-transparent while dragging
            canvasGroup.alpha = 0.7f;
            
            // Make the block pass through other UI elements (ignore raycast)
            canvasGroup.blocksRaycasts = false;
            
            if (isInGrid)
            {
                originalPosition = Position;
                BlockPuzzle.Instance.ClearBlockFromGrid(this);
            }
            
            // Bring this block to the front while dragging
            transform.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsFixed) return;
        
        // Get the current pointer position in screen space
        Vector2 pointerPosition = eventData.position;
        
        // Calculate the new position by adding the offset to the current pointer position
        Vector2 targetScreenPosition = pointerPosition + dragOffset;
        
        // Convert the screen position to world position
        Vector3 worldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform.parent as RectTransform,
            targetScreenPosition,
            eventData.pressEventCamera,
            out worldPosition);
        
        // Set the block's position
        rectTransform.position = worldPosition;
        
        // Calculate nearest grid position for preview
        Vector2Int nearestGridPos = GetGridPositionFromPointer(eventData);
        
        // Update preview position and color based on validity
        bool isValidPosition = BlockPuzzle.Instance.IsPositionValidForBlock(nearestGridPos, this);
        if (preview != null)
        {
            Vector3 previewWorldPos = GetPreviewWorldPosition(nearestGridPos);
            
            if (isValidPosition)
            {
                ShowPreview(previewWorldPos);
            }
            else
            {
                ShowPreview(previewWorldPos, false);
            }
        }
    }
    
    // Helper method to get the correct world position for the preview
    private Vector3 GetPreviewWorldPosition(Vector2Int gridPosition)
    {
        // Calculate the center position of the grid cell in world space
        Vector3 gridCellCenter = Vector3.zero;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            BlockPuzzle.Instance.gridContainer,
            RectTransformUtility.WorldToScreenPoint(
                Camera.main,
                BlockPuzzle.Instance.GridToWorldPosition(gridPosition)
            ),
            Camera.main,
            out gridCellCenter
        );
        
        return gridCellCenter;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsFixed) return;
        
        // Reset opacity and raycasting
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        // Calculate target grid position
        Vector2Int targetPosition = GetGridPositionFromPointer(eventData);
        
        if (BlockPuzzle.Instance.IsPositionValidForBlock(targetPosition, this))
        {
            // Place block in grid
            Position = targetPosition;
            
            // Convert grid position to world position
            Vector3 worldPos = GetPreviewWorldPosition(Position);
            rectTransform.position = worldPos;
            
            BlockPuzzle.Instance.SetBlockInGrid(this, Position);
            isInGrid = true;
        }
        else
        {
            // Return block to original position if it was in grid
            if (isInGrid)
            {
                Position = originalPosition;
                
                // Convert grid position to world position
                Vector3 worldPos = GetPreviewWorldPosition(Position);
                rectTransform.position = worldPos;
                
                BlockPuzzle.Instance.SetBlockInGrid(this, Position);
            }
            else
            {
                // Return to staging area if it wasn't in grid
                rectTransform.position = TransformPointToWorld(stagingPosition);
            }
        }
        
        // Hide preview
        if (preview != null)
        {
            preview.Hide();
        }
    }
    
    // Helper method to transform a position to world space
    private Vector3 TransformPointToWorld(Vector3 localPos)
    {
        // Convert from anchored position to world position
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform.parent as RectTransform,
            RectTransformUtility.WorldToScreenPoint(Camera.main, localPos),
            Camera.main,
            out worldPos
        );
        
        return worldPos;
    }

    private Vector2Int GetGridPositionFromPointer(PointerEventData eventData)
    {
        // Convert screen position to world position (accounting for the drag offset)
        Vector2 pointerPosition = eventData.position;
        Vector2 targetScreenPosition = pointerPosition + dragOffset;
        
        // First convert to world space
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            BlockPuzzle.Instance.gridContainer,
            targetScreenPosition,
            eventData.pressEventCamera,
            out worldPos
        );
        
        // Then convert to local space in the grid
        Vector3 localPos = BlockPuzzle.Instance.gridContainer.InverseTransformPoint(worldPos);
        
        // Now we can convert local position to grid position
        float cellWidth = BlockPuzzle.Instance.gridContainer.rect.width / BlockPuzzle.Instance.GridWidth;
        float cellHeight = BlockPuzzle.Instance.gridContainer.rect.height / BlockPuzzle.Instance.GridHeight;
        
        float gridX = (localPos.x + BlockPuzzle.Instance.gridContainer.rect.width / 2) / cellWidth;
        float gridY = (localPos.y + BlockPuzzle.Instance.gridContainer.rect.height / 2) / cellHeight;
        
        return new Vector2Int(
            Mathf.FloorToInt(gridX),
            Mathf.FloorToInt(gridY)
        );
    }

    private void ShowPreview(Vector3 position, bool isValid = true)
    {
        if (preview == null) return;
        
        // Set the preview position directly without additional offsets
        // The GetPreviewWorldPosition method already handles the positioning correctly
        preview.SetPosition(position);
        
        // Set color based on valid placement
        if (isValid)
        {
            preview.SetSprite(rectTransform.sizeDelta, image.sprite, image.color);
        }
        else
        {
            preview.SetSprite(rectTransform.sizeDelta, image.sprite, new Color(1f, 0.3f, 0.3f));
        }
        
        preview.Show();
    }
}
