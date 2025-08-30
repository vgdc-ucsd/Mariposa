using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class InGameUI : Singleton<InGameUI>
{
    public GameObject InteractPromptUI;
    public override void Awake()
    {
        base.Awake();
        InteractPrompt(false);
    }

    public void InteractPrompt(bool toggle)
    {
        InteractPromptUI.SetActive(toggle);
    }
    
    public void UpdateInteractPrompt()
    {
        if (DialogueManager.Instance.isPlayingDialogue)
        {
            InteractPrompt(false);
            return;
        }
        StartCoroutine(UpdateInteractPromptCoroutine());
    }

    private IEnumerator UpdateInteractPromptCoroutine()
    {
        yield return new WaitForFixedUpdate();

        IControllable controlled = PlayerController.Instance.CurrentControllable;
        bool isInsideAny = false;

        if (controlled != null)
        {
            Body controlledBody = controlled.body;
            foreach (var trigger in FindObjectsOfType<InteractionTrigger>())
            {
                trigger.EnsureControlledPlayerInside();
                if (controlled is BeeMovement &&
                    trigger.LinkedInteractable is not Switch &&
                    trigger.LinkedInteractable is not ItemPickup)
                {
                    continue;
                }
                if (controlled is SquidMovement &&
                    trigger.LinkedInteractable is not WaterPuzzleInteractable)
                {
                    continue;
                }
                if (trigger.GetIsInside(controlledBody))
                {
                    isInsideAny = true;
                }
            }
        }
        InteractPrompt(isInsideAny);
    }
}
