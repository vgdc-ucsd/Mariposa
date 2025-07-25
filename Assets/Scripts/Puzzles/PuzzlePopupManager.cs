using UnityEngine;

/// <summary>
/// Class that handles the displaying of puzzle popups
/// </summary>
public class PuzzlePopupManager : MonoBehaviour
{
    public static PuzzlePopupManager Instance;
    [SerializeField] private GameObject activePuzzle;

    public GameObject ActivePuzzle
    {
        get => activePuzzle;
        set
        {
            if (activePuzzle != null) HidePuzzle();
            activePuzzle = value;
            if (activePuzzle != null)
            {
                activePuzzle.SetActive(true);
                ShowPuzzle();
            }
        }
    }

    /// <summary>
    /// Ensure there is only one instance of PuzzleManager
    /// </summary>
    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Debug.LogWarning("Tried to create more than one instance of the PuzzleManager singleton!");
            Destroy(this);
        }
    }



    /// <summary>
    /// Try to hide the active puzzle when the player clicks on the screen.
    /// If the player clicks on a blocker panel, prevent the puzzle from closing.
    /// </summary>
    public void TryHidePuzzle()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.001f);
        foreach (Collider2D col in collider2Ds)
        {
            if (col.gameObject.CompareTag("blocker"))
            {
                return;
            }
        }

        HidePuzzle();
    }

    public void CompletePuzzle() => HidePuzzle();

    /// <summary>
    /// Hide the active puzzle
    /// </summary>
    private void HidePuzzle()
    {
        if (PlayerController.Instance != null) PlayerController.Instance.SetMovementLock(false);
        activePuzzle.SetActive(false);
    }

    /// <summary>
    /// Display a new active puzzle
    /// </summary>
    private void ShowPuzzle()
    {
        if (InGameUI.Instance != null) InGameUI.Instance.InteractPrompt(false);
        if (PlayerController.Instance) PlayerController.Instance.SetMovementLock(true);
        activePuzzle.SetActive(true);
    }
}
