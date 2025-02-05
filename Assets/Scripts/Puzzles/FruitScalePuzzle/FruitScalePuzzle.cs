using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for the Fruit Scale puzzle. The player must place fruit of varying 
/// weights onto scales so that they all balance at the same level.
/// </summary>
public class FruitScalePuzzle : Puzzle
{
    public static FruitScalePuzzle Instance;
    public const int FRUIT_MIN_WEIGHT = 1;
    public const int FRUIT_MAX_WEIGHT = 5;

    [SerializeField] private List<FruitScale> scales;
    [SerializeField] private List<Fruit> fruits; // TODO: used for debug, remove later
    private bool isComplete;

    void Awake()
    {
        if(Instance == null) Instance = this;
        else
        {
            Debug.LogWarning("Tried to create more than one instance of the FruitScalePuzzle singleton!");
            Destroy(this);
        }
    }

    void Start()
    {
        // TODO: used for debugging, remove later
        for(int i = 0; i < scales.Count; ++i)
        {
            scales[i].SetFruit(fruits[i]);
        }
        CheckScales();
    }

    /// <summary>
    /// TODO: description
    /// </summary>
    private void CheckScales()
    {
        // If the puzzle is already complete, do nothing
        // This theorietically should be unnecessary, but good for safety
        if (isComplete) return;
        
        foreach(FruitScale scale in scales)
        {
            // If any of the scales are not balanced, puzzle is not complete
            if(scale.CalcWeightDiff() != 0) return;
        }

        // If all of the scales are balanced, complete the puzzle
        isComplete = true;
        OnComplete();
    }
}
