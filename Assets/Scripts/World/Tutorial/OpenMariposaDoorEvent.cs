using UnityEngine;

public class OpenMariposaDoorEvent : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;

    void Start()
    {
        spriteRenderer.sprite = closedSprite;
    }

    public void OnRepair()
    {
        spriteRenderer.sprite = openSprite;
    }
}
