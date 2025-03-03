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

    public override void OnEnter(Body body)
    {
        base.OnEnter(body);
        InGameUI.Instance.InteractPrompt(true);
    }

    public override void OnExit(Body body)
    {
        base.OnExit(body);
        InGameUI.Instance.InteractPrompt(body.InsideTriggers.Count > 0);
    }

    public void InteractTrigger(IControllable controllable)
    {
        if (GetIsInside(controllable.body))
        {
            LinkedInteractable.OnInteract(controllable);
        }
    }
}