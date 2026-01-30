using FMOD.Studio;
using Unity.VisualScripting;
using UnityEngine;

// Trigger that mediates player interaction with the interactable if the player is inside
public class InteractionTrigger : Trigger
{
    public Interactable LinkedInteractable = null;

    public EventInstance buttonSFX;
    
    public void Start()
    {
        if (LinkedInteractable == null)
        {
            Debug.LogError("Interaction Trigger not linked to Interactable");
        }
    }

    public override bool OnEnter(Body body)
    {
        if (!base.OnEnter(body)) return false;
        if (body == PlayerController.Instance.ControlledPlayer.GetComponent<Body>())
        {
            InGameUI.Instance.InteractPrompt(true);
        }
        return true;
    }

    public override void OnExit(Body body)
    {
        base.OnExit(body);
        if (body == PlayerController.Instance.ControlledPlayer.GetComponent<Body>())
        {
            InGameUI.Instance.InteractPrompt(false);
        }
    }

    public void InteractTrigger(IControllable controllable)
    {
        Body playerBody = controllable.body;
        if (TriggerCollider != null && GetIsInside(playerBody))
        {
            LinkedInteractable.OnInteract(controllable);
            if (LinkedInteractable.DestroyOnInteract)
            {
                Destroy(LinkedInteractable.gameObject);
            }
            if(buttonSFX.isValid())
            {
                buttonSFX.start();
            }
        }
    }
    public void EnsureControlledPlayerInside()
    {
        IControllable controlledPlayer = PlayerController.Instance.CurrentControllable;
        if (controlledPlayer == null || TriggerCollider == null) return;
        Body playerBody = controlledPlayer.body;
        if (playerBody == null) return;
        Collider2D playerCollider = controlledPlayer.transform.GetComponent<Collider2D>();
        if (playerCollider == null) return;
        bool touching = TriggerCollider.IsTouching(playerCollider);
        if (touching && !GetIsInside(playerBody))
        {
            OnEnter(playerBody);
        }
        else if (!touching && GetIsInside(playerBody))
        {
            OnExit(playerBody);
        }
    }
}