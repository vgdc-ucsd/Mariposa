using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class BlockPuzzleInteractable : GenericInteractable
{
    [SerializeField] private BlockPuzzle puzzle;

    public override void OnInteract(IControllable controllable)
    {
        /* PuzzlePopupManager.Instance.ActivePuzzle = puzzle.gameObject; */
        base.OnInteract(controllable);
        RuntimeManager.PlayOneShot(AudioEvents.SFX.charging_station_click);
        SceneManager.LoadScene(2); // TODO remove after EOQ social
    }


    private void Update()
    {
        if (isActiveAndEnabled == puzzle.isActiveAndEnabled) gameObject.SetActive(!puzzle.isActiveAndEnabled);
    }
}
