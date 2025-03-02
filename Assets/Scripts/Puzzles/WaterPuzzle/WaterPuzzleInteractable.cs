using System.Collections;
using UnityEngine;

public class WaterPuzzleInteractable : Interactable
{
    public WaterPuzzle waterPuzzle;

    // This is just for testing, OnInteract will be called based on player interaction
    private new IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        OnInteract();
    }


    protected override void SetProximity()
    {

    }

    [ContextMenu("Interact")]
    public override void OnInteract()
    {
        waterPuzzle.gameObject.SetActive(true);
        PuzzlePopupManager.Instance.ActivePuzzle = waterPuzzle.gameObject;
    }
}
