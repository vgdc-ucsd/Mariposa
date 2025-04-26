using UnityEngine;

// includes switches, items, characters, etc
public abstract class Interactable : MonoBehaviour
{
    // Interactables must have an associated trigger that links to it as a child.
    // If no trigger is found, create a default one.
    public virtual bool DestroyOnInteract => false;

    protected virtual void Awake() { 
        if (GetComponentInChildren<InteractionTrigger>() == null) 
        {
            InteractionTrigger newTrigger = GameObject.Instantiate(GameManager.Instance.DefaultInteractionTrigger);
            newTrigger.transform.SetParent(transform, false);
            newTrigger.LinkedInteractable = this;
        }
    }

    protected virtual void Start() { }
    public abstract void OnInteract(IControllable controllable);
}
