using UnityEngine;
using UnityEngine.SceneManagement;

public class BlockPuzzleInteractable : GenericInteractable
{
    [SerializeField] private BlockPuzzle puzzle;

    private void Start()
    {
        // if (!puzzle.Initialized) puzzle.Initialize();
    }

    public override void OnInteract(IControllable controllable)
    {
        /* PuzzlePopupManager.Instance.ActivePuzzle = puzzle.gameObject; */
        base.OnInteract(controllable);
        SceneManager.LoadScene(2); // TODO remove after EOQ social
    }


    private void Update()
    {
        if (isActiveAndEnabled == puzzle.isActiveAndEnabled) gameObject.SetActive(!puzzle.isActiveAndEnabled);
    }
}
