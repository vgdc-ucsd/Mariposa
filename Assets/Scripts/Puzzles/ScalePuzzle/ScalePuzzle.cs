using System.Collections.Generic;
using UnityEngine;

public class ScalePuzzle : Puzzle
{
    public static ScalePuzzle Instance;
    [HideInInspector] public List<ScaleObject> scaleObjects;
    public ScaleHand[] scaleHands;
    public List<RectTransform> dragTargets = new List<RectTransform>();
    [SerializeField] private GameObject selectionArea;
    [SerializeField] private float handMoveScale;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Debug.LogWarning("Tried to make more than one instance of the ScalePuzzle singleton!");
            Destroy(this);
        }
    }

    private void Start()
    {
        foreach (ScaleHand sh in scaleHands) dragTargets.Add(sh.GetComponent<RectTransform>());
        dragTargets.Add(selectionArea.GetComponent<RectTransform>());
    }

    public void MoveHands()
    {
        float weightDiff = scaleHands[0].totalWeight - scaleHands[1].totalWeight;
        // if weightDiff > 0, first hand is heavier
        // if weightDiff < 0, second hand is heavier

        
        scaleHands[0].transform.localPosition = scaleHands[0].initialPos - Vector3.up * weightDiff * handMoveScale;
        scaleHands[1].transform.localPosition = scaleHands[1].initialPos + Vector3.up * weightDiff * handMoveScale;

    }
}
