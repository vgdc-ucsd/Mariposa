using UnityEngine;
using UnityEngine.UI;

public class InteractPrompt : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite mariposaPopup;
    [SerializeField] private Sprite unnamedPopup;

    void OnEnable()
    {
        if (PlayerController.Instance && Player.ActivePlayer.Data.characterID == CharID.Mariposa)
        {
            image.sprite = mariposaPopup;
        }
        else image.sprite = unnamedPopup;
    }
}
