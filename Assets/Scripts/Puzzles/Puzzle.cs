using UnityEngine;

/// <summary>
/// Abstract class that executes general code whenever a puzzle is completed or 
/// reset.
/// </summary>
public abstract class Puzzle : MonoBehaviour
{
    /// <summary>
    /// Executes generic puzzle completion actions. Should be called from child 
    /// class when player completes the puzzle.
    /// </summary>
    public void OnComplete()
    {
        Debug.Log("Puzzle Complete!");
    }

    /// <summary>
    /// Executes generic puzzle completion actions.
    /// </summary>
    public void Reset()
    {
        Debug.Log("Puzzle Reset");
    }
}
