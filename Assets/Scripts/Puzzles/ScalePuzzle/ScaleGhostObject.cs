using UnityEngine;
using UnityEngine.UI;

public class ScaleGhostObject : MonoBehaviour
{
    [SerializeField] private Image ghostBlockSprite;
    [SerializeField] private RectTransform ghostRectTransform;

    public void Initialize(Image blockSprite, RectTransform rt)
    {
        gameObject.SetActive(false);
        ghostBlockSprite.sprite = blockSprite.sprite;
        ghostRectTransform.sizeDelta = rt.sizeDelta;
    }
}
