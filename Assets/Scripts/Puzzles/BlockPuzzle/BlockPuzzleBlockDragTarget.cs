using UnityEngine;
using UnityEngine.EventSystems;

public class BlockPuzzleBlockDragTarget : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private BlockPuzzleBlock parentBlock;

    void Awake()
    {
        parentBlock = GetComponentInParent<BlockPuzzleBlock>();
    }

    public void OnBeginDrag(PointerEventData eventData) { parentBlock.OnBeginDrag(eventData); }
    public void OnDrag(PointerEventData eventData) { parentBlock.OnDrag(eventData); }
    public void OnEndDrag(PointerEventData eventData) { parentBlock.OnEndDrag(eventData); }
    public void OnPointerEnter(PointerEventData eventData) { parentBlock.OnPointerEnter(eventData); }
    public void OnPointerExit(PointerEventData eventData) { parentBlock.OnPointerExit(eventData); }
}
