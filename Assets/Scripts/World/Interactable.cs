using UnityEngine;

public abstract class Interactable
{
    public float Proximity { get; set; }
    // field for Interactable pop up

    // also set the Interactable pop up here    
    protected Interactable(float proximity)
    {
        Proximity = proximity;
    }

    // Check whether within proximity first
    // Load the pop up
    public abstract void OnInteract();
}
