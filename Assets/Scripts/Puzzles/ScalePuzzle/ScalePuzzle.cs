using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ScalePuzzle : Puzzle
{
    public const int maxObjects = 10;
    public static ScalePuzzle Instance;
    [SerializeField] private GameObject mysteryBoxPrefab;
    public GameObject[] scaleObjectPrefabs;
    public ScaleHand[] scaleHands;
    public List<RectTransform> dragTargets = new List<RectTransform>();
    public GameObject selectionArea;
    [SerializeField] private float handMoveScale;
    [HideInInspector] public bool isDragging = false;
    public ScalePuzzleLevel[] levels;
    private int level = 0;
    [HideInInspector] public MysteryBox mysteryBox;
    public GameObject ghost;
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
        GenerateSolution();
    }

    public void MoveHands()
    {
        int weightDiff = scaleHands[0].totalWeight - scaleHands[1].totalWeight;
        // if weightDiff > 0, first hand is heavier
        // if weightDiff < 0, second hand is heavier
        if (weightDiff != 0)
        {
            weightDiff = weightDiff / Mathf.Abs(weightDiff); // gets the +/- sign of weightDiff;

            scaleHands[0].transform.localPosition = scaleHands[0].initialPos - Vector3.up * weightDiff * handMoveScale;
            scaleHands[1].transform.localPosition = scaleHands[1].initialPos + Vector3.up * weightDiff * handMoveScale;
        }
        else
        {
            // can solve puzzle
            scaleHands[0].transform.localPosition = scaleHands[0].initialPos;
            scaleHands[1].transform.localPosition = scaleHands[1].initialPos;
        }
        

    }

    /// <summary>
    /// Called to start the next level
    /// </summary>
    public void GenerateSolution()
    {
        mysteryBox = Instantiate(mysteryBoxPrefab, selectionArea.transform).GetComponent<MysteryBox>();
        Vector3 offset = Vector3.up * selectionArea.GetComponent<RectTransform>().rect.height / 2.5f;
        mysteryBox.transform.localPosition += offset;
        foreach (ScaleObject scaleObj in levels[level].objects)
        {
            mysteryBox.mysteryObjects.Add(scaleObj);
            mysteryBox.weight += scaleObj.weight;
        }

        mysteryBox.text.text = mysteryBox.mysteryObjects.Count.ToString();



    }

    public void CheckSolution()
    {
        bool boxOnOneSide = scaleHands[0].scaleObjects.Contains(mysteryBox) && scaleHands[0].scaleObjects.Count == 1;
        boxOnOneSide |= scaleHands[1].scaleObjects.Contains(mysteryBox) && scaleHands[1].scaleObjects.Count == 1;

        if (boxOnOneSide && scaleHands[0].totalWeight == scaleHands[1].totalWeight)
        {
            if (level < levels.Length - 1)
            {
                ResetPuzzle();
                TryHidePuzzle();
                DialogueManager.Instance.PlayDialogue(levels[level].dialogue);
                level++;
                GenerateSolution();
            }
            else
            {
                OnComplete();
            }
        }
    }

    private void ResetPuzzle()
    {
        foreach (ScaleHand scaleHand in scaleHands)
        {
            foreach (ScaleObject scaleObj in scaleHand.scaleObjects) Destroy(scaleObj.gameObject);
            scaleHand.scaleObjects.Clear();
            scaleHand.totalWeight = 0;
            scaleHand.UpdateWeightText();
        }
        if (mysteryBox != null) Destroy(mysteryBox.gameObject);
        
    }

}
