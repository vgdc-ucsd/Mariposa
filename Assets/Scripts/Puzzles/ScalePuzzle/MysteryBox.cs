using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MysteryBox : ScaleObject
{
    public List<ScaleObject> mysteryObjects = new List<ScaleObject>();
    public TMP_Text text;
    [HideInInspector] public Vector3 defaultPos; // in case the player manages to lose the box
    private new void Start()
    {
        base.Start();
        defaultPos = GetComponent<RectTransform>().localPosition;
    }
}
