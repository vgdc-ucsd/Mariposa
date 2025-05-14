using UnityEngine;
using UnityEngine.EventSystems;

public class BlockPuzzleSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Vector2Int GridPosition;

    public void OnPointerEnter(PointerEventData eventData)
    {
        BlockPuzzle.Instance.HoveredSlot = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BlockPuzzle.Instance.HoveredSlot = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // if (eventData.pointerDrag.TryGetComponent<BlockPuzzleBlock>(out var block))
        // {
        //     BlockPuzzle.Instance.SetBlockInGrid(block, GridPosition);
        // }
    }
}
