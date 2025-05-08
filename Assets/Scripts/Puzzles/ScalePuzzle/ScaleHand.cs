using System.Collections.Generic;
using UnityEngine;

public class ScaleHand : MonoBehaviour
{
    [HideInInspector] public List<ScaleObject> scaleObjects = new List<ScaleObject>();
    public const int maxFruits = 10;
    [SerializeField] private Vector3[] stackPositions;
    [HideInInspector] public float totalWeight;
    [HideInInspector] public Vector3 initialPos;

    private void Start()
    {
        totalWeight = 0f;
        initialPos = GetComponent<RectTransform>().localPosition;
    }

    public void AddObject(ScaleObject obj)
    {
        scaleObjects.Add(obj);
        totalWeight += obj.weight;
    }

    public void RemoveObject(ScaleObject obj)
    {
        scaleObjects.Remove(obj);
        totalWeight -= obj.weight;
    }
}
