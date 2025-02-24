using UnityEngine;

public class BlockPreview : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }

    public void SetSize(Vector2Int size)
    {
        transform.localScale = new Vector3(size.x, size.y, 1);
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
