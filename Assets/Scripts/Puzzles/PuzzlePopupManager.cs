using UnityEngine;

/// <summary>
/// Class that handles the displaying of puzzle popups
/// </summary>
public class PuzzlePopupManager : MonoBehaviour
{
    public static PuzzlePopupManager Instance;

    private GameObject activePuzzle;
    public GameObject ActivePuzzle
    {
        get => activePuzzle;
        set {
            if(activePuzzle != null) HidePuzzle();
            activePuzzle = value;
            if (activePuzzle != null) ShowPuzzle();
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
        Debug.Log("Tried to hide puzzle");


        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.001f);
        foreach(Collider2D col in collider2Ds)
        {
            if(col.gameObject.CompareTag("blocker"))
            {
                Debug.Log("Clicked blocker");
                Debug.Log(col.gameObject.name);
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
        activePuzzle.SetActive(false);
    }

    /// <summary>
    /// Display a new active puzzle
    /// </summary>
    private void ShowPuzzle()
    {
        activePuzzle.SetActive(true);
    }
}
