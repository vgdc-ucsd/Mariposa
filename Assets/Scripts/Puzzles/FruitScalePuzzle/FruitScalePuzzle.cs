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

    [SerializeField] private Fruit selectedFruit;
    public Fruit SelectedFruit
    {
        get => selectedFruit;
        set
        {
            if(selectedFruit != null) selectedFruit.Deselect();
            selectedFruit = value;
            if(selectedFruit != null)
            {
                selectedFruit.Select();
                foreach(FruitScale scale in scales)
                {
                    scale.BoxCollider2D.enabled = true;
                }
            }
            else
            {
                foreach(FruitScale scale in scales)
                {
                    scale.BoxCollider2D.enabled = false;
                }
            }
        }
    }
    public FruitScale OldScale;
    public bool IsComplete;

    [SerializeField] private List<FruitScale> scales;
    [SerializeField] private List<Fruit> fruits;
    private List<int> weights = new();

    /// <summary>
    /// Ensure there is only one instance of the puzzle's singleton.
    /// Initialize random values, set initial placements for fruits and scales.
    /// </summary>
    void Awake()
    {
        if(Instance == null) Instance = this;
        else
        {
            Debug.LogWarning("Tried to create more than one instance of the FruitScalePuzzle singleton!");
            Destroy(this);
        }

        if(scales.Count != fruits.Count)
        {
            Debug.LogWarning("Invalid number of fruits and scales");
        }

        // Create array of possible weights
        int newWeight;
        for(int i = 0; i < fruits.Count; ++i)
        {
            newWeight = Random.Range(FRUIT_MIN_WEIGHT, FRUIT_MAX_WEIGHT);
            weights.Add(newWeight);
            fruits[i].Weight = newWeight;
            scales[i].ScaleValue = newWeight;
        }

        // TODO: debug print generated weights, remove later
        string weightString = "{ ";
        foreach(int i in weights)
        {
            weightString += i + " ";
        }
        Debug.Log("weights: " + weightString + "}");

        // Randomize fruit starting scales
        foreach(FruitScale scale in scales)
        {
            int index = Random.Range(0, fruits.Count);
            Fruit fruit = fruits[index];
            scale.Fruit = fruit;
            fruit.Scale = scale;
            fruits.Remove(fruit);
            scale.InitializeScale();
        }
    }

    /// <summary>
    /// When the player moves a fruit, update references and visual positions
    /// </summary>
    /// <param name="newScale">The scale the fruit was moved to</param>
    public void PlaceFruit(FruitScale newScale)
    {
        // If the puzzle is already complete, do nothing
        // This theorietically should be unnecessary, but good for safety
        if(IsComplete) return;

        // Update references for fruit and scales
        Fruit otherFruit = newScale.Fruit; // For directly swapping
        otherFruit.Scale = OldScale;
        OldScale.Fruit = otherFruit;
        SelectedFruit.Scale = newScale;
        newScale.Fruit = SelectedFruit;
        SelectedFruit = null;

        // Update vertical positions for old and new scales
        if(!newScale.IsStorage) StartCoroutine(newScale.LerpPositionY());
        if(!OldScale.IsStorage) StartCoroutine(OldScale.LerpPositionY());

        // Check if puzzle is solved after each action
        if(CheckScales())
        {
            // TODO: trigger after final scale finishes moving into its balanced position place?
            // Currently triggers instantly when the last fruit is placed on its scale
            IsComplete = true;
            OnComplete();
        }
    }
    
    /// <summary>
    /// Check if all scales are properly balanced
    /// </summary>
    /// <returns>True if all scales are balanced, false otherwise</returns>
    private bool CheckScales()
    {
        foreach(FruitScale scale in scales)
        {
            // If any of the scales are not balanced, puzzle is not complete
            if(scale.CalcWeightDiff() != 0) return false;
        }

        // If all of the scales are balanced, the puzzle is solved
        return true;
    }
}
