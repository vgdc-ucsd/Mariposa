using UnityEngine;
using UnityEngine.EventSystems;

public class BlockPuzzleSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector2Int GridPos;

    public void OnPointerEnter(PointerEventData eventData)
    {
        BlockPuzzle.Instance.HoveredSlot = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BlockPuzzle.Instance.HoveredSlot = null;
    }
}
