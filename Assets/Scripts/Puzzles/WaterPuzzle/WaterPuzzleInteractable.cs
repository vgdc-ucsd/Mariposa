using System.Collections;
using UnityEngine;

public class WaterPuzzleInteractable : Interactable
{
    public WaterPuzzle waterPuzzle;


    [ContextMenu("Interact")]
    public override void OnInteract(IControllable controllable)
    {
        waterPuzzle.gameObject.SetActive(true);
        PuzzlePopupManager.Instance.ActivePuzzle = waterPuzzle.gameObject;
    }
}
