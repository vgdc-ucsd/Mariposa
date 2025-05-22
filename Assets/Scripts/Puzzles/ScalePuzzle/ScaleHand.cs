using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScaleHand : MonoBehaviour
{
    [HideInInspector] public List<ScaleObject> scaleObjects = new List<ScaleObject>();
    [SerializeField] private Vector3[] stackPositions;
    [HideInInspector] public int totalWeight;
    [HideInInspector] public Vector3 initialPos;
    [SerializeField] private RectTransform platform;
    [SerializeField] private TMP_Text weightText;
    private Rect platformRect;

    private Vector3[] corners;

    private void Start()
    {
        totalWeight = 0;
        initialPos = GetComponent<RectTransform>().localPosition;
        platformRect = platform.rect;
        weightText.text = string.Empty;
    }

    

    public void AddObject(ScaleObject obj)
    {
        scaleObjects.Add(obj);
        totalWeight += obj.weight;
        UpdateWeightText();
        Debug.Log(obj.GetComponent<RectTransform>().localPosition.x);
        Debug.Log(platformRect.x);
        FitToPlatform(obj.GetComponent<RectTransform>(), true);
        ScalePuzzle.Instance.CheckSolution();
        
    }

    public void RemoveObject(ScaleObject obj)
    {
        scaleObjects.Remove(obj);
        totalWeight -= obj.weight;
        UpdateWeightText();
        ScalePuzzle.Instance.CheckSolution();
    }

    public void UpdateWeightText()
    {
        if (scaleObjects.Contains(ScalePuzzle.Instance.mysteryBox)) weightText.text = "???";
        else weightText.text = (totalWeight == 0 ? string.Empty : totalWeight.ToString());
    }

    public void FitToPlatform(RectTransform objRect, bool print = false)
    {
        objRect.SetParent(platform, true);
        if (print) Debug.Log(objRect.localPosition.x);
        objRect.transform.localPosition = new Vector3(
            Mathf.Clamp(objRect.transform.localPosition.x, platformRect.x, platformRect.xMax),
            Mathf.Clamp(objRect.transform.localPosition.y, platformRect.y, platformRect.yMax), 0
        );
    }
}
