using System.Collections.Generic;
using UnityEngine;

public class ScalePuzzle : Puzzle
{
    [HideInInspector] public ScaleHand TargetScale;
    [SerializeField] private ScaleHand leftHand;
    [SerializeField] private ScaleHand rightHand;
    [SerializeField] private float handMoveScale;
    private int totalNumBlocks;

    void Start()
    {
        totalNumBlocks = GetComponentsInChildren<ScaleObject>().Length;
    }

    public void MoveHands()
    {
        float weightDiff = (leftHand.TotalWeight - rightHand.TotalWeight) * handMoveScale;
        leftHand.CalculatePosition(weightDiff);
        rightHand.CalculatePosition(-weightDiff);
    }

    public void DropBlock(ScaleObject block)
    {
        if (TargetScale == null)
        {
            block.ReturnToOrigin();
            return;
        }

        block.Scale?.RemoveObject(block);
        TargetScale.AddObject(block);
        MoveHands();
        CheckSolution();
    }

    public void ShowGhost(ScaleGhostObject ghostBlock)
    {
        if (TargetScale == null)
        {
            ghostBlock.gameObject.SetActive(false);
            return;
        }

        ghostBlock.gameObject.SetActive(true);
        TargetScale.FitToPlatform(ghostBlock.transform);
    }

    public void CheckSolution()
    {
        bool balanced = leftHand.TotalWeight == rightHand.TotalWeight;
        bool allBlocksUsed = leftHand.NumBlocks + rightHand.NumBlocks == totalNumBlocks;
        if (balanced && allBlocksUsed) OnComplete();
    }
}
