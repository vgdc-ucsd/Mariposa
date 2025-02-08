using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public float Proximity { get; set; }
    // field for Interactable pop up

    protected virtual void Start()
    {
        SetProximity();
    }

    // Check whether within proximity first
    // Load the pop up
    public abstract void OnInteract();
    protected abstract void SetProximity();
}
