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

    public Fruit SelectedFruit;
    public FruitScale OldScale;
    public bool IsComplete;

    [SerializeField] private List<FruitScale> scales;

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
    }

    /// <summary>
    /// When the puzzle is enabled, initialize all scales' initial positions
    /// </summary>
    void OnEnable()
    {
        foreach(FruitScale scale in scales) {
            scale.SetPositionY();
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

        SelectedFruit.SetScale(newScale);
        newScale.SetFruit(SelectedFruit);
        Instance.OldScale.SetFruit(null);

        // Visually move fruit to new scale position
        SelectedFruit.transform.position = newScale.transform.position;
        SelectedFruit.Deselect();
        SelectedFruit = null;

        // Update vertical positions for old and new scales
        if(!newScale.IsStorage()) StartCoroutine(newScale.LerpPositionY());
        if(!Instance.OldScale.IsStorage()) StartCoroutine(Instance.OldScale.LerpPositionY());

        // Prepare scale colliders for next action
        Instance.OldScale.GetComponent<BoxCollider2D>().enabled = true;
        newScale.GetComponent<BoxCollider2D>().enabled = false;

        // Check if puzzle is solved after each action
        if(CheckScales())
        {
            // TODO: this currently triggers instantly when the last fruit is placed on its scale
            // Should trigger after final scale finishes moving into its balanced position place
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
