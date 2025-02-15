using UnityEngine;

public class WaterPuzzleInteractable : Interactable
{
    public WaterPuzzle waterPuzzle;

    protected override void SetProximity()
    {

    }

    [ContextMenu("Interact")]
    public override void OnInteract()
    {
        waterPuzzle.gameObject.SetActive(true);
    }
}
