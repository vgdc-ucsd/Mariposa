using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public float Proximity { get; set; }
    // field for Interactable pop up

    protected virtual void Start()
    {
        SetProximity();
    }

    public abstract void OnInteract();
    protected abstract void SetProximity();
}
