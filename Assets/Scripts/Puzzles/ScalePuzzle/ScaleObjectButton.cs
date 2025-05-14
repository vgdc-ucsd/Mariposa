using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScaleObjectButton : MonoBehaviour
{
    [SerializeField] private int index;
    private ScaleObject scaleObject;
    private Vector3 mousePos;
    private RectTransform rectTransform;

    private void Start()
    {
        scaleObject = ScalePuzzle.Instance.scaleObjectPrefabs[index].GetComponent<ScaleObject>();
        GetComponent<Image>().sprite = scaleObject.GetComponent<Image>().sprite;
        GetComponentInChildren<TMP_Text>().text = scaleObject.weight.ToString();
        rectTransform = GetComponent<RectTransform>();
    }
    private void Update()
    {
        mousePos = RectTransformUtility.WorldToScreenPoint(Camera.main, Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (Input.GetMouseButtonDown(0))
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePos))
            {
                OnClick();
            }
        }
    }
    public void OnClick()
    {
        ScaleObject obj = Instantiate(scaleObject.gameObject, transform.position, Quaternion.identity).GetComponent<ScaleObject>();
        obj.transform.SetParent(ScalePuzzle.Instance.selectionArea.transform, true);
        obj.OnClick();
    }
}