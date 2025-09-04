using UnityEngine;

public class DebugManager : Singleton<DebugManager>
{
    private InputSystem_Actions inputs;

    public override void Awake()
    {
        inputs = new InputSystem_Actions();
        base.Awake();
    }

    public void Start()
    {
        if (!GameManager.Instance.Debug.DebugEnabled)
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        inputs.Enable();
        inputs.Debug.CompletePuzzle.performed += ctx => DebugCompletePuzzle();
        inputs.Debug.AdvanceLevel.performed += ctx => DebugGoToNextLevel();
        inputs.Debug.AdvanceSublevel.performed += ctx => DebugGoToNextSublevel();
    }

    void OnDisable()
    {
        inputs.Debug.CompletePuzzle.performed -= ctx => DebugCompletePuzzle();
        inputs.Debug.AdvanceLevel.performed -= ctx => DebugGoToNextLevel();
        inputs.Debug.AdvanceSublevel.performed -= ctx => DebugGoToNextSublevel();
        inputs.Disable();
    }

    private void DebugCompletePuzzle()
    {
        PuzzlePopupManager.Instance?.ActivePuzzle?.GetComponent<Puzzle>().OnComplete();
    }

    private void DebugGoToNextLevel()
    {
        LevelManager.Instance?.LoadNextLevel();
    }

    private void DebugGoToNextSublevel()
    {
        LevelManager.Instance?.GoToNextSublevel();
    }
}
