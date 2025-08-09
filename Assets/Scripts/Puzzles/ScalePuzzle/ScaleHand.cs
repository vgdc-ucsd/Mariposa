using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScaleHand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ScalePuzzle scalePuzzle;
    public int TotalWeight { get; private set; }
    public int NumBlocks { get; private set; }
    [SerializeField] private RectTransform platform;
    [SerializeField] private GameObject arrow;
    [SerializeField] private float arrowRotateScale;
    private Vector3 initialPos;

    private void Start()
    {
        scalePuzzle = GetComponentInParent<ScalePuzzle>();
        TotalWeight = 0;
        NumBlocks = 0;
        initialPos = GetComponent<RectTransform>().localPosition;
    }

    public void CalculatePosition(float weightDiff)
    {
        transform.localPosition = initialPos - Vector3.up * weightDiff;
    }

    public void UpdateWeight(int weight)
    {
        TotalWeight += weight;
        arrow.transform.eulerAngles = Mathf.Sqrt(TotalWeight * arrowRotateScale) * Vector3.forward;
    }

    public void AddObject(ScaleObject block)
    {
        NumBlocks++;
        UpdateWeight(block.Weight);
        FitToPlatform(block.transform);
        block.Scale = this;
    }

    public void RemoveObject(ScaleObject block)
    {
        NumBlocks--;
        UpdateWeight(-block.Weight);
    }

    public void FitToPlatform(Transform block)
    {
        block.SetParent(platform, true);
        Vector2 pos = platform.position;
        Vector2 rectMinWorld = platform.rect.min + pos;
        Vector2 rectMaxWorld = platform.rect.max + pos;
        block.position = new Vector3
        (
            Mathf.Clamp(block.position.x, rectMinWorld.x, rectMaxWorld.x),
            Mathf.Clamp(block.position.y, rectMinWorld.y, rectMaxWorld.y),
            0.0f
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        scalePuzzle.TargetScale = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        scalePuzzle.TargetScale = null;
    }
}
