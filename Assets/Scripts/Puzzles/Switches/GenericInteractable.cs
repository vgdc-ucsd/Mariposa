using UnityEngine;

public class GenericInteractable : Interactable
{
    public override void OnInteract(IControllable controllable)
    {
        Debug.Log(controllable.body.name);
    }
}