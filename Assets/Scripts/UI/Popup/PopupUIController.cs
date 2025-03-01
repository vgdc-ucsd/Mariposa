using UnityEngine;

public class PopupUIController : MonoBehaviour
{
    [SerializeField] PopupUI popupUIGroup;


    private void Start()
    {
        Hide();
    }

    private void Update()
    {
        // Test
        if (Input.GetKeyDown(KeyCode.R))
        {
            PopupDataSO testData = new();
            testData.Description = "This is a description of a test item";
            Show(testData);
		}

        if (Input.GetKeyDown(KeyCode.E))
        {
            Hide();
		}
    }

    public void Show(PopupDataSO aData)
    {
        SetPopupData(aData);
        popupUIGroup.gameObject.SetActive(true);
	}

    public void Hide()
    {
        popupUIGroup.gameObject.SetActive(false);
	}

    public void SetPopupData(PopupDataSO aData)
    {
        popupUIGroup.SetPopupUI(aData);
	}

}
