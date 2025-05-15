using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleHand : MonoBehaviour
{
    [HideInInspector] public List<ScaleObject> scaleObjects = new List<ScaleObject>();
    [SerializeField] private Vector3[] stackPositions;
    [HideInInspector] public int totalWeight;
    [HideInInspector] public Vector3 initialPos;
    [SerializeField] private RectTransform platform;
    private Rect platformRect;

    private Vector3[] corners;

    private void Start()
    {
        totalWeight = 0;
        initialPos = GetComponent<RectTransform>().localPosition;
        platformRect = platform.rect;
    }

    

    public void AddObject(ScaleObject obj)
    {
        scaleObjects.Add(obj);
        totalWeight += obj.weight;
        Debug.Log(obj.GetComponent<RectTransform>().localPosition.x);
        Debug.Log(platformRect.x);
        FitToPlatform(obj.GetComponent<RectTransform>(), true);
        ScalePuzzle.Instance.CheckSolution();
        
    }

    public void RemoveObject(ScaleObject obj)
    {
        scaleObjects.Remove(obj);
        totalWeight -= obj.weight;
        ScalePuzzle.Instance.CheckSolution();
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
