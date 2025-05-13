using UnityEngine;
using UnityEngine.UI;

public class BlockPreview : MonoBehaviour
{
    private const float BLOCK_PREVIEW_ALPHA = 0.25f;
    private Image image;
    private RectTransform rectTransform;

    void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    // public void SetSize(Vector2Int size)
    // {
    //     transform.localScale = new Vector3(size.x, size.y, 1);
    // }

    public void SetSprite(Vector2 sizeDelta, Sprite sprite, Color color)
    {
        image.sprite = sprite;
        image.color = new Color(color.r, color.g, color.b, BLOCK_PREVIEW_ALPHA);
        rectTransform.sizeDelta = sizeDelta;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
