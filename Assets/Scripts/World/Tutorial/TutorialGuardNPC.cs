using System.Collections.Generic;
using UnityEngine;

public class TutorialGuardNPC : NPC
{
    [SerializeField] private ItemData keycard;
    [SerializeField] private GameObject turnstileInteractable;
    private bool initialInteract = true;
    private bool turnstileFixed = false;

    private const string INITIAL_DIALOGUE = "guard";
    private const string BEFORE_FIXING = "interact_guard";
    private const string AFTER_FIXING = "guard_post_puzzle";

    protected override void Start()
    {
        base.Start();
        turnstileInteractable.SetActive(false);
    }

    protected override string GetDialogue()
    {
        if (initialInteract)
        {
            initialInteract = false;
            turnstileInteractable.SetActive(true);
            InventoryManager.Instance.GetInventory().AddItem(keycard);
            return INITIAL_DIALOGUE;
        }

        if (!turnstileFixed) return BEFORE_FIXING;
        else return AFTER_FIXING;
    }

    public void SetTurnstileFixed()
    {
        turnstileFixed = true;
    }
}
