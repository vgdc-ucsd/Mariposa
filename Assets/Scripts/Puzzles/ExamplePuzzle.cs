using UnityEngine;

/// <summary>
/// An example puzzle demonstrating a simple implementation of completing and 
/// resetting a puzzle.
/// </summary>
public class ExamplePuzzle : Puzzle
{
    public static ExamplePuzzle Instance;
    private bool testCondition;
    private bool isComplete;

    /// <summary>
    /// Ensure there is only one instance of the puzzle's singleton
    /// </summary>
    void Awake()
    {
        if(Instance == null) Instance = this;
        else
        {
            // Update warning message with your puzzle class name
            Debug.LogWarning("Tried to create more than one instance of the ExamplePuzzle singleton!");
            Destroy(this);
        }
    }

    void Start()
    {
        // Activated on start for debug purposes, but this can be called from 
        // the Inspector, such as for a button press or hitbox detection
        PlayerDoesSomething();
    }

    /// <summary>
    /// Handles player action, then checks if the player has met all conditions 
    /// to complete the puzzle.
    /// </summary>
    private void PlayerDoesSomething()
    {
        // If the puzzle is already complete, do nothing
        // This theorietically should be unnecessary, but good for safety
        if (isComplete) return;

        Debug.Log("Player did something");
        testCondition = true;

        // If the player has met all conditions, complete the puzzle
        if (testCondition)
        {
            isComplete = true;
            OnComplete();
        }
    }
}
