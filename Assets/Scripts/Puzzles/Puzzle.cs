using UnityEngine;
using FMODUnity;

/// <summary>
/// Abstract class that executes general code whenever a puzzle is completed or 
/// reset.
/// </summary>
public abstract class Puzzle : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Executes generic puzzle completion actions. Should be called from child 
    /// class when player completes the puzzle.
    /// </summary>
    public void OnComplete()
    {
        Debug.Log("Puzzle Complete!");
        if (PuzzlePopupManager.Instance != null) PuzzlePopupManager.Instance.CompletePuzzle();
        else Debug.Log("No PuzzlePopupManager found");

        if (Player.ActivePlayer.Character.Id == CharID.Mariposa)
        {
            RuntimeManager.PlayOneShot("event:/sfx/puzzle/puzzle_complete/mariposa");
        }
        else
        {
            RuntimeManager.PlayOneShot("event:/sfx/puzzle/puzzle_complete/unnamed");
        }
    }

    /// <summary>
    /// Executes generic puzzle completion actions.
    /// </summary>
    public void Reset()
    {
        Debug.Log("Puzzle Reset");
    }

    public void TryHidePuzzle()
    {
        PuzzlePopupManager.Instance.TryHidePuzzle();
    }
}
