using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupUI : MonoBehaviour
{
    public Image Icon;
    public TMP_Text Description;

    public void SetPopupUI(PopupDataSO aData)
    {
        Icon.sprite = aData.IconSprite;
        Description.text = aData.Description;
	}

}
