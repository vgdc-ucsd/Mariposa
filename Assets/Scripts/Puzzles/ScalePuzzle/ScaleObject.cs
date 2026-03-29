using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScaleObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    [HideInInspector] public ScaleHand Scale; 
    [field: SerializeField] public int Weight { get; private set; }
    [SerializeField] private ScaleGhostObject ghostBlock;
    [SerializeField] private Image blockSprite;
    [SerializeField] private RectTransform rectTransform;
    private ScalePuzzle scalePuzzle;
    private Vector3 shelfOrigin;
    private Vector3 origin;
    private Transform originalParent;

    void Start()
    {
        scalePuzzle = GetComponentInParent<ScalePuzzle>();
        originalParent = transform.parent;
        shelfOrigin = transform.position;
        ghostBlock.Initialize(blockSprite, rectTransform);
    }

    public void ReturnToOrigin()
    {
        transform.position = origin;
    }

    public void ReturnToShelf()
    {
        transform.SetParent(originalParent);
        transform.position = shelfOrigin;
        Scale?.RemoveObject(this);
        Scale = null;
        scalePuzzle.MoveHands();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        ghostBlock.transform.position = eventData.position;
        scalePuzzle.ShowGhost(ghostBlock);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        blockSprite.raycastTarget = false;
        origin = transform.position;
        RuntimeManager.PlayOneShot(AudioEvents.SFX.block_pickup);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        blockSprite.raycastTarget = true;
        ghostBlock.gameObject.SetActive(false);
        scalePuzzle.DropBlock(this);
        RuntimeManager.PlayOneShot(AudioEvents.SFX.block_place);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ReturnToShelf();
        }
    }
}
