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

    private Fruit selectedFruit;
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
                    scale.Collider2D.enabled = true;
                }
            }
            else
            {
                foreach(FruitScale scale in scales)
                {
                    scale.Collider2D.enabled = false;
                }
            }
        }
    }
    public FruitScale OldScale;
    public bool IsComplete;

    [SerializeField] private List<FruitScale> scales;
    [SerializeField] private List<Fruit> fruits;

    /// <summary>
    /// Ensure there is only one instance of the puzzle's singleton
    /// </summary>
    void Awake()
    {
        if(Instance == null) Instance = this;
        else
        {
            Debug.LogWarning("Tried to create more than one instance of the FruitScalePuzzle singleton!");
            Destroy(this);
        }

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

        // TODO: randomize scale locations
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
        otherFruit.Scale = Instance.OldScale;
        Instance.OldScale.Fruit = otherFruit;
        SelectedFruit.Scale = newScale;
        newScale.Fruit = SelectedFruit;
        SelectedFruit = null;

        // Update vertical positions for old and new scales
        if(!newScale.IsStorage) StartCoroutine(newScale.LerpPositionY());
        if(!Instance.OldScale.IsStorage) StartCoroutine(Instance.OldScale.LerpPositionY());

        // Prepare scale colliders for next action
        Instance.OldScale.Collider2D.enabled = true;
        newScale.Collider2D.enabled = false;

        // Check if puzzle is solved after each action
        if(CheckScales())
        {
            // TODO: trigger after final scale finishes moving into its balanced position place
            // Currently triggers instantly when the last fruit is placed on its scale
            IsComplete = true;
            OnComplete();
        }
    }

    /// <summary>
    /// Check if all scales are properly balanced
    /// </summary>
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
