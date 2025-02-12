using UnityEngine;

/// <summary>
/// Class to handle opening the popup window for a puzzle
/// </summary>
public class PuzzlePopupButton : MonoBehaviour
{
    [SerializeField] private GameObject puzzleObject;

    /// <summary>
    /// Activates the puzzle when the player clicks on the object.
    /// This should later be replaced with the interact keybind when close to object
    /// </summary>
    void OnMouseDown()
    {
        // TODO: replace mouse click with interact keybind
        Debug.Log("Clicked popup");
        PuzzlePopupManager.Instance.ActivePuzzle = puzzleObject;
    }
}
