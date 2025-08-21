using Unity.VisualScripting;
using UnityEngine;

// Trigger that mediates player interaction with the interactable if the player is inside
public class InteractionTrigger : Trigger
{
    public Interactable LinkedInteractable = null;
    
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
        }
    }
    public void EnsureControlledPlayerInside()
    {
        Player controlledPlayer = PlayerController.Instance.ControlledPlayer;
        if (controlledPlayer == null || TriggerCollider == null) return;
        Body playerBody = controlledPlayer.GetComponent<Body>();
        if (playerBody == null) return;
        Collider2D playerCollider = controlledPlayer.GetComponent<Collider2D>();
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