using UnityEngine;

public class ChargingStation : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite chargedSprite;
    [SerializeField] private Animator botAnimator;
    private InteractionTrigger interactionTrigger;

    private const string CHARGE_ANIMATION = "Charge";
    private const string CHARGE_DIALOGUE = "block_puzzle_complete";

    void Start()
    {
        interactionTrigger = GetComponentInChildren<InteractionTrigger>();
        interactionTrigger.gameObject.SetActive(false);
    }

    public void SetCharged()
    {
        spriteRenderer.sprite = chargedSprite;
        interactionTrigger.gameObject.SetActive(true);
    }

    public void OnRepair()
    {
        botAnimator.SetTrigger(CHARGE_ANIMATION);
        DialogueManager.Instance.PlayDialogue(CHARGE_DIALOGUE);
    }
}
